using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System;

public enum PlayerCharacterType
{
    ShotGun,
    Dash,
    Rocket
}


[RequireComponent(typeof(GravityObjectRigidBody))]
public abstract class PlayerController : NetworkBehaviour {
    private static short _aimingReticleIdCnt = 0;

    public GravityObjectRigidBody AttachedObject;

    [SerializeField]
    protected float DurationOfNormalGravityProjectile = 1;
    [SerializeField]
    protected float DurationOfShotgunGravityProjectile = .25f;
    [SerializeField]
    protected float DurationOfRocketGravityProjectile = 1.5f;

    [SerializeField]
    protected float MoveSpeed = 1f;
    [SerializeField]
    protected float ShootSpeed = 1f;
    [SerializeField]
    protected float JumpForce = 10f;
    [SerializeField]
    protected float ImpulseToKill = 10f;
    [SerializeField]
    protected float DashSpeed = 10f;
    [SerializeField]
    protected float DashDurationTime = .25f;
    [SerializeField]
    protected float ShotGunKickBackForce = 10f;
    [SerializeField]
    protected GameObject ProjectilePrefab;
    [SerializeField]
    protected GameObject ShotGunProjectilePrefab;
    [SerializeField]
    protected GameObject RocketProjectilePrefab;
    [SerializeField]
    protected GameObject AimingReticlePrefab;
    [SerializeField]
    public GameObject Reticle;
    [SerializeField]
    protected GameObject ReticleParent;
    [SerializeField]
    protected GameObject PlayerReadyIndicatorPrefab;
    [SerializeField]
    protected DashEffect de;
    [SerializeField]
    protected GravityChange gc;

    protected readonly Vector2[] _gravChangeDirections = { Vector2.up, Vector2.down };

    public InputDevice InputDevice;

    //sound effects
    public AudioSource[] channels = new AudioSource[4]; //ch0 - ggfire, ch1 - dash/sgfire/rlaunch, ch2 - changeGrav, ch3 - death
    public AudioClip gravChange;
    public AudioClip death;
    public AudioClip dash;
    public AudioClip[] rocketLaunch;
    public AudioClip[] rocketLaunchCave;
    public AudioClip[] gravGunFire;
    public AudioClip[] gravGunFireCave;
    public AudioClip[] shotGunFire;
    public AudioClip[] shotGunFireCave;

    public Action OnHitObjectWithNormalProjectile;

    private static int _heartBeatId = 0;
    //THIS SHOULD ONLY BE USED FROM HEART BEAT FUNCTION
    public Dictionary<int, bool> HeartBeats = new Dictionary<int, bool>();

    private List<GameObject> GravityGunProjectiles = new List<GameObject>();
    private Coroutine GravGunCoolDownCoroutine;
    private PlayerCooldownController _cooldownController;

    [SyncVar]
    public short ReticleId = -1;
    [SyncVar]
    public short MaterialId = -1;
    [SyncVar]
    public bool IsReady = false;
    [SyncVar]
    public Player ControlledPlayer;
    [SyncVar]
    public bool IsDead;
    [SyncVar]
    public bool IsInvincible;
    [SyncVar(hook = "ChangeMaterial")]
    public PlayerCharacterType CharacterType;

    protected virtual void Awake()
    {
        IsDead = false;
        _cooldownController = GetComponent<PlayerCooldownController>();
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(FindReticle());
        var indicator = Instantiate(PlayerReadyIndicatorPrefab);
        indicator.GetComponent<PlayerReadyIndicatorController>().AttachedPlayer = this;
        de = GetComponentInChildren<DashEffect>();
        gc = GetComponentInChildren<GravityChange>();
        GetComponent<Renderer>().material = GetComponent<CharacterSelectController>().CharacterTypeMaterialMappings[CharacterType];
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        GameObject aimingReticle = Instantiate(AimingReticlePrefab);
        _aimingReticleIdCnt++;
        aimingReticle.GetComponent<AimingReticle>().Id = _aimingReticleIdCnt;
        aimingReticle.GetComponent<AimingReticle>().PlayerAttachedTo = netId;
        ReticleId = _aimingReticleIdCnt;

        GetComponent<Renderer>().material = GetComponent<CharacterSelectController>().CharacterTypeMaterialMappings[CharacterType];

        NetworkServer.SpawnWithClientAuthority(aimingReticle, connectionToClient);
        ReticleParent = gameObject;

        ControlledPlayer.NumLives = LevelSelectManager.Instance.numLivesPerPlayer;
    }

    public override void OnStartLocalPlayer()
    {
        StartCoroutine(AttachInputDeviceToPlayer());
        LevelManager.Instance.SpawnPlayerDestructive(this);
        GetComponent<GravityObjectRigidBody>().CanMove = false;
    }

    IEnumerator AttachInputDeviceToPlayer()
    {
        GetComponent<GravityObjectRigidBody>().IsSimulatedOnThisConnection = isLocalPlayer;

        if (isLocalPlayer)
        {
            if (ControllerSelectManager.Instance.DevicesWaitingForPlayer.Count > 0)
            {
                InputDevice = ControllerSelectManager.Instance.DevicesWaitingForPlayer[0];
                ControllerSelectManager.Instance.DevicesWaitingForPlayer.RemoveAt(0);
            }
            else
            {
                yield return new WaitForEndOfFrame();
                yield return AttachInputDeviceToPlayer();
            }
        }
    }

    public virtual void Init(Player player, Transform spawnPosition)
    {
        ControlledPlayer = player;
        transform.position = spawnPosition.transform.position;
        ChangeGORBGravityDirection(GetComponent<GravityObjectRigidBody>(), FindDirToClosestWall());
    }

    public Vector2 FindDirToClosestWall()
    {
        int layerMask = LayerMask.GetMask("Wall");
        var upHit = Physics2D.Raycast(transform.position, Vector2.up, layerMask);
        var downHit = Physics2D.Raycast(transform.position, Vector2.down, layerMask);
        if (upHit.distance < downHit.distance)
        {
            return Vector2.up;
        }
        else
        {
            return Vector2.down;
        }
    }

    public void Move(Vector2 dir)
    {
        var fixedDir = Vector2.Perpendicular(GetComponent<GravityObjectRigidBody>().GravityDirection);
        fixedDir = new Vector2(Mathf.Abs(fixedDir.x), Mathf.Abs(fixedDir.y));
       
        GetComponent<GravityObjectRigidBody>().UpdateVelocity(VelocityType.Movement, Vector3.Project(dir, fixedDir) * MoveSpeed);
    }

    public void FlipGravity()
    {
        if (isLocalPlayer)
        {
            if (!_cooldownController.IsCoolingDown(CooldownType.ChangeGravity))
            {
                ChangeGravity(GetComponent<GravityObjectRigidBody>().GravityDirection * -1);
                gc.PlayEffect(GetComponent<GravityObjectRigidBody>());
            }
        }
        else
        {
            CmdBroadcastFlipGravity();
        }
    }

    public void ChangeGravityTowardsDir(Vector2 dir)
    {
        var GORB = GetComponent<GravityObjectRigidBody>();
        var compass = new List<Vector2> { GORB.GravityDirection, -GORB.GravityDirection };
        ChangeGravity(ClosestDirection(dir, compass.ToArray()));
        gc.PlayEffect(GetComponent<GravityObjectRigidBody>());
    }

    [Command]
    public void CmdBroadcastFlipGravity()
    {
        RpcBroadcastFilpGravity();
    }

    [ClientRpc]
    public void RpcBroadcastFilpGravity()
    {
        if (isLocalPlayer)
        {
            FlipGravity();
        }
    }

    public void ChangeGravity(Vector2 dir)
    {
        if (!_cooldownController.IsCoolingDown(CooldownType.ChangeGravity))
        {
            ChangeGORBGravityDirection(GetComponent<GravityObjectRigidBody>(), dir);
            _cooldownController.StartCooldown(CooldownType.ChangeGravity, () => { });
            PlaySingle(gravChange, 2);
        }
    }

    public void Jump()
    {
        throw new System.NotImplementedException();
    }

    public void Dash(Vector2 dir)
    {
        if (!_cooldownController.IsCoolingDown(CooldownType.Dash))
        {
            de.dashOn = true;
            //need to account for gravity
            var dashVec = dir.normalized * DashSpeed;

            var GORB = GetComponent<GravityObjectRigidBody>();
            var compass = new List<Vector2> { GORB.GravityDirection, -GORB.GravityDirection };
            ChangeGORBGravityDirection(GORB, ClosestDirection(dir, compass.ToArray(), GORB.GravityDirection));

            GetComponent<GravityObjectRigidBody>().Dash(dashVec, DashDurationTime);
            PlaySingle(dash,1);

            _cooldownController.StartCooldown(CooldownType.Dash, () => { de.dashOn = false;});
        }

    }

    private IEnumerator FindReticle()
    {
        if (ReticleId != -1 && !Reticle)
        {
            //look for matching reticle
            var tempReticle = FindObjectsOfType<AimingReticle>().ToList().Find(x => x.Id == ReticleId);
            if (tempReticle)
            {
                Reticle = tempReticle.gameObject;
                if (!ReticleParent)
                {
                    ReticleParent = gameObject;
                }
            }
        }

        yield return new WaitForEndOfFrame();
        yield return (FindReticle());
    }

    public void AimReticle(Vector2 dir)
    {
        if (isLocalPlayer)
        {
            if(!ReticleParent)
            {
                ReticleParent = gameObject;
            }

            if (Reticle)
            {
                var normalizedDir = dir.normalized;
                Reticle.transform.position = ReticleParent.transform.position + new Vector3(normalizedDir.x, normalizedDir.y, 0);
            }
        }
    }

    public void ShootGravityGun(Vector2 dir, ProjectileControllerType type)
    {
        if (isLocalPlayer && !IsDead)
        {
            dir = dir.normalized;

            if (type == ProjectileControllerType.Normal)
            {
                if (!_cooldownController.IsCoolingDown(CooldownType.NormalShot))
                {
                    if (AttachedObject == null)
                    {

                        CmdSpawnProjectile(dir, DurationOfNormalGravityProjectile, ProjectileControllerType.Normal);
                        RandomizeSfx(gravGunFire, gravGunFireCave, 0);
                        _cooldownController.StartCooldown(CooldownType.NormalShot, () => { });
                    }
                    else
                    {
                        ChangeGORBGravityDirection(AttachedObject, dir);
                        AttachedObject.GetComponent<ControllableGravityObjectRigidBody>().LaunchSfx();
                        AttachedObject.GetComponent<ConnectionToPlayerController>().DisconnectPlayer();
                        DetachReticle();
                    }
                }
            }
            else if (type == ProjectileControllerType.Shotgun)
            {
                if (!_cooldownController.IsCoolingDown(CooldownType.ShotGunShot))
                {
                    ShotGunBlast(dir);
                    _cooldownController.StartCooldown(CooldownType.ShotGunShot, () => { });
                }
            }
            else if (type == ProjectileControllerType.Rocket)
            {
                if (!_cooldownController.IsCoolingDown(CooldownType.Rocket))
                {
                    CmdSpawnProjectile(dir, DurationOfRocketGravityProjectile, ProjectileControllerType.Rocket);
                    RandomizeSfx(rocketLaunch, rocketLaunchCave, 1);
                    _cooldownController.StartCooldown(CooldownType.Rocket, () => { });
                }
            }
        }
    }

    private void ShotGunBlast(Vector2 dir)
    {
        CmdSpawnProjectile(dir, DurationOfShotgunGravityProjectile, ProjectileControllerType.Shotgun);
        CmdSpawnProjectile(Quaternion.Euler(0, 0, 30) * new Vector3(dir.x, dir.y, 0), DurationOfShotgunGravityProjectile, ProjectileControllerType.Shotgun);
        CmdSpawnProjectile(Quaternion.Euler(0, 0, -30) * new Vector3(dir.x, dir.y, 0), DurationOfShotgunGravityProjectile, ProjectileControllerType.Shotgun);
        CmdSpawnProjectile(Quaternion.Euler(0, 0, 15) * new Vector3(dir.x, dir.y, 0), DurationOfShotgunGravityProjectile, ProjectileControllerType.Shotgun);
        CmdSpawnProjectile(Quaternion.Euler(0, 0, -15) * new Vector3(dir.x, dir.y, 0), DurationOfShotgunGravityProjectile, ProjectileControllerType.Shotgun);
        GetComponent<GravityObjectRigidBody>().Dash(-dir * ShotGunKickBackForce, _cooldownController.GetCooldownTime(CooldownType.ShotGunShot) * .25f);  
        RandomizeSfx(shotGunFire, shotGunFireCave, 1);    
    }

    [Command]
    public void CmdSpawnProjectile(Vector2 dir, float secondsUntilDestroy, ProjectileControllerType type)
    {
        float xValue = dir.x;
        float yVlaue = dir.y;
        float angle = Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x);
        GravityGunProjectileController projectileClone = ProjectilePool.Instance.GetProjectile(ProjectilePool.ConvertProjectileControllerTypeToType(type));
        projectileClone.transform.position = transform.position + new Vector3(dir.x, dir.y, 0);
        projectileClone.Owner = this;
        projectileClone.SecondsUntilDestroy = secondsUntilDestroy;
        ChangeGORBGravityDirection(projectileClone.GetComponent<GravityObjectRigidBody>(), dir);
        projectileClone.GetComponent<GravityObjectRigidBody>().ChangeGravityScale(ShootSpeed);
        projectileClone.GetComponent<GravityObjectRigidBody>().ClearAllVelocities();
        projectileClone.transform.Rotate(0, 0, angle);
        projectileClone.OnShot();
    }

    #region attach reticle
    public void AttachReticle(GravityObjectRigidBody gravityObjectRB)
    {
        if (isLocalPlayer)
        {
            AttachReticleInternal(gravityObjectRB);
        }
        else
        {
            CmdBroadCastAttachReticle(gravityObjectRB.netId);
        }
    }

    [Command]
    public void CmdBroadCastAttachReticle(NetworkInstanceId gravityObjectId)
    {
        if (isLocalPlayer)
        {
            AttachReticleInternal(NetworkServer.FindLocalObject(gravityObjectId).GetComponent<GravityObjectRigidBody>());
        }
        else
        {
            RpcBroadCastAttachReticle(gravityObjectId);
        }
    }

    [ClientRpc]
    public void RpcBroadCastAttachReticle(NetworkInstanceId gravityObjectId)
    {
        if (isLocalPlayer)
        {
            AttachReticleInternal(ClientScene.FindLocalObject(gravityObjectId).GetComponent<GravityObjectRigidBody>());
        }
    }

    public void AttachReticleInternal(GravityObjectRigidBody gravityObjectRB)
    {
        AttachedObject = gravityObjectRB;
        ReticleParent = AttachedObject.gameObject;
    }
    #endregion

    public void DetachReticle()
    {
        AttachedObject = null;
        ReticleParent = gameObject;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float otherMass = 1;
        if (collision.rigidbody)
        {
            otherMass = collision.rigidbody.mass;
        }

        var impulse = (collision.relativeVelocity * otherMass).magnitude;
        var GORB = collision.collider.GetComponent<GravityObjectRigidBody>();
        if (impulse > ImpulseToKill && GORB && GORB.KillsPlayer && isServer)
        {
            if(collision.collider.GetComponent<PlayerController>())
            {
                if(_cooldownController.IsCoolingDown(CooldownType.Dash) && !collision.collider.GetComponent<PlayerController>().IsInvincible)
                {
                    ControlledPlayer.NumKills++;
                    ControlledPlayer.NumOverallKills++;
                    SetDirtyBit(0xFFFFFFFF);
                    //kill the other player
                    collision.collider.GetComponent<PlayerController>().Kill();
                    Recoil();
                    return;
                }

                //dont kill us if we run into another player
                return;
            }

            if(_cooldownController.IsCoolingDown(CooldownType.Dash))
            {
                //if we dash into an object push it
                var dashVel = GetComponent<Rigidbody2D>().velocity;
                ChangeGORBGravityDirection(GORB, dashVel.normalized);
                Recoil();
                _cooldownController.StopCooldown(CooldownType.Dash);
                return;
            }

            //the rest happens if we run into an object 
            if(IsInvincible)
            {
                //dont kill us if we are invincible
                return;
            }
            
            if(GORB is ControllableGravityObjectRigidBody && 
                (GORB as ControllableGravityObjectRigidBody).LastShotBy != NetworkInstanceId.Invalid &&
                (GORB as ControllableGravityObjectRigidBody).LastShotBy != netId) 
            {
                var otherPlayer = NetworkServer.FindLocalObject((GORB as ControllableGravityObjectRigidBody).LastShotBy).GetComponent<PlayerController>();
                otherPlayer.ControlledPlayer.NumKills++;
                otherPlayer.ControlledPlayer.NumOverallKills++;
                otherPlayer.SetDirtyBit(0xFFFFFFFF);
            }

            Kill();
        }
    }

    //must be on server
    private void Recoil()
    {
        var GORB = GetComponent<GravityObjectRigidBody>();
        var vel = GetComponent<Rigidbody2D>().velocity;
        RpcClearVelocities(gameObject);
        RpcUpdateGORBVelocity(gameObject,VelocityType.OtherPhysics, -vel * 10);
        var compass = new List<Vector2> { GORB.GravityDirection, -GORB.GravityDirection };
        
        ChangeGORBGravityDirection(GORB, ClosestDirection(-vel, compass.ToArray(), GORB.GravityDirection));
    }

    public static Vector2 ClosestDirection(Vector2 v, Vector2[] compass)
    {
        var maxDot = -Mathf.Infinity;
        var ret = Vector3.zero;

        foreach (var dir in compass)
        {
            var t = Vector3.Dot(v, dir);
            if (t > maxDot)
            {
                ret = dir;
                maxDot = t;
            }
        }
        
        return ret;
    }

    public static Vector2 ClosestDirection(Vector2 v, Vector2[] compass, Vector3 defaultDir, float threshold = .1f)
    {
        v = v.normalized;
        var maxDot = -Mathf.Infinity;
        var ret = Vector3.zero;

        foreach (var dir in compass)
        {
            var t = Vector3.Dot(v, dir);
            if (t > maxDot)
            {
                ret = dir;
                maxDot = t;
            }
        }
        
        if (maxDot < threshold)
        {
            return defaultDir;
        }

        return ret;
    }

    public virtual void Kill()
    {
        CmdKill();
    }

    public void CmdKill()
    {
        if(LevelManager.Instance.PlayersCanDieInThisLevel)
        {
            ControlledPlayer.NumDeaths++;
            ControlledPlayer.NumOverallDeaths++;
        }
        
        SetDirtyBit(0xFFFFFFFF);
        if (isLocalPlayer)
        {
            InternalKill();
        }
        else
        {
            RpcKill();
        }
    }

    [ClientRpc]
    public void RpcKill()
    {
        if(isLocalPlayer)
        {
            InternalKill();
        }
    }

    private void InternalKill()
    {
        PlaySingle(death, 3);

        if (ControlledPlayer.NumDeaths >= ControlledPlayer.NumLives)
        {
            IsDead = true;
            transform.position = LevelManager.Instance.JailTransform.position;
        }
        else
        {
            //respawning player
            LevelManager.Instance.SpawnPlayer(this);
            ChangeInvincibility(true);
            _cooldownController.StartCooldown(CooldownType.Invincibility, () => { ChangeInvincibility(false); });
        }
    }

    private void ChangeInvincibility(bool isInvincible)
    {
        IsInvincible = isInvincible;
        CmdChangeInvincibility(isInvincible);
    }

    [Command]
    private void CmdChangeInvincibility(bool isInvincible)
    {
        IsInvincible = isInvincible;
    }

    public void DestroyAllGravGunProjectiles()
    {
        GravityGunProjectiles.ForEach(x => Destroy(x));
        GravityGunProjectiles.Clear();
    }

    public void ToggleReady()
    {
        CmdToggleReady();
    }

    [Command]
    public void CmdToggleReady()
    {
        IsReady = !IsReady;
    }

    public void UpdateGORBVelocity(GravityObjectRigidBody GORB, VelocityType type, Vector2 dir)
    {
        if (GORB.IsSimulatedOnThisConnection)
        {
            GORB.UpdateVelocity(type, dir);
        }
        else
        {
            CmdUpdateGORBVelocity(GORB.gameObject, type, dir);
        }
    }

    [Command]
    public void CmdUpdateGORBVelocity(GameObject GORB, VelocityType type, Vector2 dir)
    {
        if (GORB.GetComponent<GravityObjectRigidBody>().IsSimulatedOnThisConnection)
        {
            GORB.GetComponent<GravityObjectRigidBody>().UpdateVelocity(type, dir);
        }
        else
        {
            RpcUpdateGORBVelocity(GORB, type, dir);
        }
    }

    [ClientRpc]
    public void RpcUpdateGORBVelocity(GameObject GORB, VelocityType type, Vector2 dir)
    {
        if (GORB.GetComponent<GravityObjectRigidBody>().IsSimulatedOnThisConnection)
        {
            GORB.GetComponent<GravityObjectRigidBody>().UpdateVelocity(type, dir);
        }
    }

    #region changGORB gravity dir
    public void ChangeGORBGravityDirection(GravityObjectRigidBody GORB, Vector2 dir)
    {
        if (GORB.IsSimulatedOnThisConnection)
        {
            GORB.ChangeGravityDirectionInternal(dir);
        }
        else
        {
            CmdChangeGORBGravityDirection(GORB.gameObject, dir);
        }
    }

    [Command]
    public void CmdChangeGORBGravityDirection(GameObject GORB, Vector2 dir)
    {
        if (GORB.GetComponent<GravityObjectRigidBody>().IsSimulatedOnThisConnection)
        {
            GORB.GetComponent<GravityObjectRigidBody>().ChangeGravityDirectionInternal(dir);
        }
        else
        {
            RpcChangeGORBGravityDirection(GORB, dir);
        }
    }

    [ClientRpc]
    public void RpcChangeGORBGravityDirection(GameObject GORB, Vector2 dir)
    {
        if (GORB.GetComponent<GravityObjectRigidBody>().IsSimulatedOnThisConnection)
        {
            GORB.GetComponent<GravityObjectRigidBody>().ChangeGravityDirectionInternal(dir);
        }
    }
    #endregion

    [ClientRpc]
    public void RpcClearVelocities(GameObject GORB)
    {
        if (GORB.GetComponent<GravityObjectRigidBody>().IsSimulatedOnThisConnection)
        {
            GORB.GetComponent<GravityObjectRigidBody>().ClearAllVelocities();
        }
    }

    public void InitializeForStartLevel(Vector3 spawnPoint, bool isDead)
    {
        if (isLocalPlayer)
        {
            InitializeForStartLevelInternal(spawnPoint, isDead);
        }
        else
        {
            CmdInitializeForStartLevel(spawnPoint, isDead);
        }
    }

    public void InitializeForStartLevelInternal(Vector3 spawnPoint, bool isDead)
    {
        transform.position = spawnPoint;
        GetComponent<GravityObjectRigidBody>().ClearAllVelocities();
        ChangeGORBGravityDirection(GetComponent<GravityObjectRigidBody>(), FindDirToClosestWall());
        IsDead = isDead;
    }

    [Command]
    public void CmdInitializeForStartLevel(Vector3 spawnPoint, bool isDead)
    {
        if (isLocalPlayer)
        {
            InitializeForStartLevelInternal(spawnPoint, isDead);
        }
        else
        {
            RpcInitializeForStartLevel(spawnPoint, isDead);
        }
    }

    [ClientRpc]
    public void RpcInitializeForStartLevel(Vector3 spawnPoint, bool isDead)
    {
        if (isLocalPlayer)
        {
            InitializeForStartLevelInternal(spawnPoint, isDead);
        }
    }

    //call this to make sure that a command is synced up
    //MUST BE CALLED FROM SERVER
    public int GetHeartBeat()
    {
        int heartBeatId = _heartBeatId++;
        if(isLocalPlayer)
        {
            HeartBeats[heartBeatId] = true;
        }
        else
        {
            HeartBeats[heartBeatId] = false;
            RpcHeartBeat(heartBeatId);
        }

        return heartBeatId;
    }

    [Command]
    public void CmdHeartBeat(int heartBeatId)
    {
        HeartBeats[heartBeatId] = true;
    }

    [ClientRpc]
    public void RpcHeartBeat(int heartBeatId)
    {
        CmdHeartBeat(heartBeatId);
    }

    protected void DoSpecial(Vector2 aimVector)
    {
        switch (CharacterType)
        {
            case PlayerCharacterType.ShotGun:
                ShootGravityGun(aimVector, ProjectileControllerType.Shotgun);
                break;
            case PlayerCharacterType.Rocket:
                ShootGravityGun(aimVector, ProjectileControllerType.Rocket);
                break;
            case PlayerCharacterType.Dash:
                Dash(aimVector);
                break;
        }
    }

    public void ChangeMaterial(PlayerCharacterType characterType)
    {
        if(GetComponent<CharacterSelectController>())
        {
            GetComponent<Renderer>().material = GetComponent<CharacterSelectController>().CharacterTypeMaterialMappings[characterType];
            this.CharacterType = characterType;
        }
    }

    public void PlaySingle(AudioClip clip, int channel)
    {
        //Set the clip of our efxSource audio source to the clip passed in as a parameter.
        channels[channel].clip = clip;

        //Play the clip.
        channels[channel].Play();
    }


    //RandomizeSfx chooses randomly between various audio clips
    public void RandomizeSfx(AudioClip[] clips, AudioClip[] caveClips, int channel)
    {
        //Generate a random number between 0 and the length of our array of clips passed in.
        int randomIndex;
        if (AudioManager.instance.isCaveLevel)
            randomIndex = UnityEngine.Random.Range(0, caveClips.Length);
        else
            randomIndex = UnityEngine.Random.Range(0, clips.Length);

        //Choose a random pitch to play back our clip at between our high and low pitch ranges.
        float randomPitch = UnityEngine.Random.Range(AudioManager.instance.lowPitchRange, AudioManager.instance.highPitchRange);

        //Set the pitch of the audio source to the randomly chosen pitch.
        channels[channel].pitch = randomPitch;

        //Set the clip to the clip at our randomly chosen index.
        if (AudioManager.instance.isCaveLevel)
            channels[channel].clip = caveClips[randomIndex];
        else
            channels[channel].clip = clips[randomIndex];
        channels[channel].volume = 1.0f * AudioManager.SFXVol * AudioManager.MasterVol;
        //Play the clip.
        channels[channel].Play();
    }
}