﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public enum PlayerCharacterType
{
    ShotGun,
    Dash,
    Rocket
}


[RequireComponent(typeof(GravityObjectRigidBody))]
public abstract class PlayerController : MonoBehaviour
{
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
    public Transform AimingReticleCenter;
    [SerializeField]
    protected GameObject ReticleParent;
    [SerializeField]
    protected GameObject PlayerReadyIndicatorPrefab;
    [SerializeField]
    protected DashEffect de;
    [SerializeField]
    protected GravityChange gc;
    [SerializeField]
    protected GameObject character;
    [SerializeField]
    protected GameObject characterContainer;
    [SerializeField]
    public LightingLookAt Lightning;

    protected deatheffect dth;
    protected readonly Vector2[] _gravChangeDirections = { Vector2.up, Vector2.down };

    public InputDevice InputDevice;

    //sound effects
    public AudioSource[] channels = new AudioSource[4]; //ch0 - ggfire, ch1 - dash/sgfire/rlaunch, ch2 - changeGrav, ch3 - death, ch4 -death indicator
    public AudioSource gg_grab;
    public AudioClip gravChange;
    public AudioClip death;
    public AudioClip dash;
    public AudioClip[] rocketLaunch;
    public AudioClip[] rocketLaunchCave;
    public AudioClip[] gravGunFire;
    public AudioClip[] gravGunFireCave;
    public AudioClip[] shotGunFire;
    public AudioClip[] shotGunFireCave;
    public AudioClip[] deathIndicator;
    public Transform deathLocation;

    public Action OnHitObjectWithNormalProjectile;

    private List<GameObject> GravityGunProjectiles = new List<GameObject>();
    private Coroutine GravGunCoolDownCoroutine;
    private PlayerCooldownController _cooldownController;
    static List<short> PlayerIdPool = new List<short>() { 1, 2, 3, 4 };
    public short PlayerId = -1;
    public short ReticleId = -1;
    public short MaterialId = -1;
    public bool IsReady = false;
    public Player ControlledPlayer;
    public bool IsDead;
    public bool IsInvincible;
    public PlayerCharacterType CharacterType;
    public GameObject PlayerReadyIndicator;

    public delegate void OnDestroyDelegate();
    public OnDestroyDelegate OnDestroyCallback;

    protected virtual void Awake()
    {
        IsDead = false;
        _cooldownController = GetComponent<PlayerCooldownController>();
        PlayerId = PlayerIdPool[0];
        PlayerIdPool.RemoveAt(0);
        OnDestroyCallback += () => { };
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        PlayerReadyIndicator = Instantiate(PlayerReadyIndicatorPrefab);
        PlayerReadyIndicator.GetComponent<PlayerReadyIndicatorController>().AttachedPlayer = this;
        
        de = GetComponentInChildren<DashEffect>();
        gc = GetComponentInChildren<GravityChange>();
        dth = GetComponentInChildren<deatheffect>();
        
        if (GameManager.Instance.OnPlayerJoin != null)
        {
            GameManager.Instance.OnPlayerJoin(this);
        }

        ControlledPlayer.NumLives = GameRoundSettingsController.Instance.NumLivesPerRound;

        StartCoroutine(AttachInputDeviceToPlayer());
        LevelManager.Instance.SpawnPlayer(this);
    }

    IEnumerator AttachInputDeviceToPlayer()
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

    //to be used from external and ui operations, checks for specific states to ignore the drop
    public void DropPlayer()
    {
        if (SceneManagementController.Instance.IsCurrentlyTransitioning() || !GameManager.Instance.IsInCharacterSelect)
        {
            return;
        }
        
        DropPlayerInternal();
    }

    public void DropPlayerInternal()
    {
        ControllerSelectManager.Instance.RemoveUsedDevice(InputDevice);
        
        if(AttachedObject)
        {
            DetachReticle();
        }

        OnDestroyCallback();

        PlayerIdPool.Add(PlayerId);
        Debug.Log(_aimingReticleIdCnt);
        Destroy(Reticle);
        Destroy(Lightning.LObject);
        Destroy(PlayerReadyIndicator);
        Destroy(gameObject);
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
        if (!_cooldownController.IsCoolingDown(CooldownType.ChangeGravity))
        {
            ControlledPlayer.GravityChanges++;
            ChangeGravity(GetComponent<GravityObjectRigidBody>().GravityDirection * -1);
            gc.PlayEffect(GetComponent<GravityObjectRigidBody>());
            Animator currentAnimator = GetComponent<CharacterAnimScript>().currentAnimator;
            currentAnimator.SetTrigger("Flip");
        }
    }

    public void ChangeGravityTowardsDir(Vector2 dir)
    {
        var GORB = GetComponent<GravityObjectRigidBody>();
        var compass = new List<Vector2> { GORB.GravityDirection, -GORB.GravityDirection };
        ChangeGravity(ClosestDirection(dir, compass.ToArray()));
        gc.PlayEffect(GetComponent<GravityObjectRigidBody>());

        var rotY = 180f;
        if (GetComponent<GravityObjectRigidBody>().GravityDirection.y < 0)
        {
            rotY = 0;
            Animator currentAnimator = GetComponent<CharacterAnimScript>().currentAnimator;
            currentAnimator.SetTrigger("Flip");
        }
    }

    public void ChangeGravity(Vector2 dir)
    {
        if (!_cooldownController.IsCoolingDown(CooldownType.ChangeGravity))
        {
            ChangeGORBGravityDirection(GetComponent<GravityObjectRigidBody>(), dir);
            _cooldownController.StartCooldown(CooldownType.ChangeGravity, () => { });
            PlaySingle(gravChange, 2);

                Animator currentAnimator = GetComponent<CharacterAnimScript>().currentAnimator;
            currentAnimator.SetTrigger("Flip");
        }
    }

    public void Jump()
    {
        throw new System.NotImplementedException();
    }

    public void Dash(Vector2 dir)
    {
        if (!_cooldownController.IsCoolingDown(CooldownType.Dash) && !_cooldownController.IsCoolingDown(CooldownType.DashRecharge))
        {
            de.dashOn = true;
            //need to account for gravity
            var dashVec = dir.normalized * DashSpeed;

            var GORB = GetComponent<GravityObjectRigidBody>();
            var compass = new List<Vector2> { GORB.GravityDirection, -GORB.GravityDirection };
            ChangeGORBGravityDirection(GORB, ClosestDirection(dir, compass.ToArray(), GORB.GravityDirection));
            PlaySingle(dash, 1);

            GORB.ClearAllVelocities();
            GORB.ChangeGravityScale(0);
            GORB.UpdateVelocity(VelocityType.Dash, dashVec);
            _cooldownController.StartCooldown(CooldownType.Dash, () => 
            {
                GORB.ClearAllVelocities();
                GORB.ChangeGravityScale(1);
                de.dashOn = false;
                _cooldownController.StartCooldown(CooldownType.DashRecharge, () => { });
            });
        }

    }

    public void AimReticle(Vector2 dir)
    {
        if (!ReticleParent)
        {
            ReticleParent = AimingReticleCenter.gameObject;
        }

        if (Reticle)
        {
            var normalizedDir = dir.normalized;
            Reticle.transform.position = ReticleParent.transform.position;
            Reticle.transform.position += new Vector3(normalizedDir.x, normalizedDir.y, 0);
            //set animation details
            Animator currentAnimator = GetComponent<CharacterAnimScript>().currentAnimator;
            currentAnimator.SetFloat("Horizontal", normalizedDir.x);
            if (GetComponent<GravityObjectRigidBody>().GravityDirection.y < 0)
            {
                currentAnimator.SetFloat("Vertical", normalizedDir.y);
            }
            else
            {
                currentAnimator.SetFloat("Vertical", -normalizedDir.y);
            }
        }
    }

    public void ShootGravityGun(Vector2 dir, ProjectileControllerType type)
    {
        if (!IsDead)
        {
            dir = dir.normalized;

            if (type == ProjectileControllerType.Normal)
            {
                ControlledPlayer.ShotsFired++;
                if (!_cooldownController.IsCoolingDown(CooldownType.NormalShot))
                {
                    gg_grab.volume = AudioManager.MasterVol * AudioManager.SFXVol;
                    if (AttachedObject == null)
                    {
                        SpawnProjectile(dir, DurationOfNormalGravityProjectile, ProjectileControllerType.Normal);
                        RandomizeSfx(gravGunFire, gravGunFireCave, 0);
                        _cooldownController.StartCooldown(CooldownType.NormalShot, () => { });
                    }
                    else
                    {
                        ChangeGORBGravityDirection(AttachedObject, dir);
                        var CGORB = AttachedObject.GetComponent<ControllableGravityObjectRigidBody>();
                        AttachedObject.GetComponent<ControllableGravityObjectRigidBody>().AttachedPlayer = null;
                        AttachedObject.GetComponent<ControllableGravityObjectRigidBody>().LaunchSfx();
                        DetachReticle();
                        CGORB.OnShot(this, CGORB);
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
                    SpawnProjectile(dir, DurationOfRocketGravityProjectile, ProjectileControllerType.Rocket);
                    RandomizeSfx(rocketLaunch, rocketLaunchCave, 1);
                    _cooldownController.StartCooldown(CooldownType.Rocket, () => { });
                }
            }
        }
    }

    private void ShotGunBlast(Vector2 dir)
    {
        SpawnProjectile(dir, DurationOfShotgunGravityProjectile, ProjectileControllerType.Shotgun);
        SpawnProjectile(Quaternion.Euler(0, 0, 30) * new Vector3(dir.x, dir.y, 0), DurationOfShotgunGravityProjectile, ProjectileControllerType.Shotgun);
        SpawnProjectile(Quaternion.Euler(0, 0, -30) * new Vector3(dir.x, dir.y, 0), DurationOfShotgunGravityProjectile, ProjectileControllerType.Shotgun);
        SpawnProjectile(Quaternion.Euler(0, 0, 15) * new Vector3(dir.x, dir.y, 0), DurationOfShotgunGravityProjectile, ProjectileControllerType.Shotgun);
        SpawnProjectile(Quaternion.Euler(0, 0, -15) * new Vector3(dir.x, dir.y, 0), DurationOfShotgunGravityProjectile, ProjectileControllerType.Shotgun);

        RandomizeSfx(shotGunFire, shotGunFire, 1);

        var GORB = GetComponent<GravityObjectRigidBody>();
        GORB.ClearAllVelocities();
        GORB.ChangeGravityScale(0);
        GORB.UpdateVelocity(VelocityType.Dash, -dir * ShotGunKickBackForce);
        _cooldownController.StartCooldown(CooldownType.ShotgunKnockback, () =>
        {
            GORB.ClearAllVelocities();
            GORB.ChangeGravityScale(1);
        });
    }

    public void SpawnProjectile(Vector2 dir, float secondsUntilDestroy, ProjectileControllerType type)
    {
        float xValue = dir.x;
        float yVlaue = dir.y;
        float angle = Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x);
        GravityGunProjectileController projectileClone = ProjectilePool.Instance.GetProjectile(ProjectilePool.ConvertProjectileControllerTypeToType(type));
        projectileClone.transform.position = Reticle.transform.position;
        projectileClone.Owner = this;
        projectileClone.SecondsUntilDestroy = secondsUntilDestroy;
        ChangeGORBGravityDirection(projectileClone.GetComponent<GravityObjectRigidBody>(), dir);
        projectileClone.GetComponent<GravityObjectRigidBody>().ChangeGravityScale(ShootSpeed);
        projectileClone.GetComponent<GravityObjectRigidBody>().ClearAllVelocities();
        projectileClone.transform.rotation = Quaternion.identity;
        projectileClone.transform.Rotate(0, 0, angle);
        projectileClone.OnShot();
    }

    public void AttachReticle(GravityObjectRigidBody gravityObjectRB)
    {
        AttachedObject = gravityObjectRB;
        ReticleParent = AttachedObject.gameObject;
        //Reticle.transform.SetParent(ReticleParent.transform);
    }

    public void DetachReticle()
    {
        AttachedObject = null;
        ReticleParent = AimingReticleCenter.gameObject;
        //Reticle.transform.SetParent(ReticleParent.transform);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float otherMass = 1;
        deathLocation = transform;
        deathLocation.position = this.transform.position;
        if (collision.rigidbody)
        {
            otherMass = collision.rigidbody.mass;
        }

        var impulse = (collision.relativeVelocity * otherMass).magnitude;
        var GORB = collision.collider.GetComponent<GravityObjectRigidBody>();
        if (impulse > ImpulseToKill && GORB && GORB.KillsPlayer)
        {
            if (collision.collider.GetComponent<PlayerController>())
            {
                if (_cooldownController.IsCoolingDown(CooldownType.Dash) && !collision.collider.GetComponent<PlayerController>().IsInvincible)
                {
                    ControlledPlayer.NumKills++;
                    ControlledPlayer.NumOverallKills++;
                    //kill the other player
                    collision.collider.GetComponent<PlayerController>().Kill();
                    Recoil();
                    return;
                }

                //dont kill us if we run into another player
                return;
            }

            if (_cooldownController.IsCoolingDown(CooldownType.Dash))
            {
                //if we dash into an object push it
                var dashVel = GetComponent<Rigidbody2D>().velocity;
                ChangeGORBGravityDirection(GORB, dashVel.normalized);
                Recoil();
                _cooldownController.StopCooldown(CooldownType.Dash);
                return;
            }

            //the rest happens if we run into an object 
            if (IsInvincible)
            {
                //dont kill us if we are invincible
                return;
            }

            if (GORB is ControllableGravityObjectRigidBody &&
                (GORB as ControllableGravityObjectRigidBody).LastShotBy)
            {
                var otherPlayer = (GORB as ControllableGravityObjectRigidBody).LastShotBy;
                otherPlayer.ControlledPlayer.NumKills++;
                otherPlayer.ControlledPlayer.NumOverallKills++;
            }

            Kill();
        }
    }

    //must be on server
    private void Recoil()
    {
        var GORB = GetComponent<GravityObjectRigidBody>();
        var vel = GetComponent<Rigidbody2D>().velocity;
        ClearVelocities(gameObject);
        UpdateGORBVelocity(GORB, VelocityType.OtherPhysics, -vel * 10);
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
        if(IsInvincible)
        {
            return;
        }

        if (LevelManager.Instance.PlayersCanDieInThisLevel)
        {
            ControlledPlayer.NumDeaths++;
            ControlledPlayer.NumOverallDeaths++;
        }

        PlaySingle(death, 3);
        RandomizeSfx(deathIndicator, deathIndicator, 4);
        IsInvincible = true;
        
        GetComponent<deatheffect>().PlayDeathEffect();

        GetComponent<GravityObjectRigidBody>().CanMove = false;
        
        _cooldownController.StartCooldown(CooldownType.Death, () => 
        {
            if (ControlledPlayer.NumDeaths >= ControlledPlayer.NumLives)
            {
                IsDead = true;
                transform.position = LevelManager.Instance.JailTransform.position;
            }
            else
            {
                dth.isDead = true;
                //respawning player
                LevelManager.Instance.SpawnPlayer(this);
            }

            GetComponent<GravityObjectRigidBody>().CanMove = true;
            _cooldownController.StartCooldown(CooldownType.Invincibility, () => { IsInvincible = false; });
        });
    }

    public void DestroyAllGravGunProjectiles()
    {
        GravityGunProjectiles.ForEach(x => Destroy(x));
        GravityGunProjectiles.Clear();
    }

    public void ToggleReady()
    {
        IsReady = !IsReady;
    }

    public void UpdateGORBVelocity(GravityObjectRigidBody GORB, VelocityType type, Vector2 dir)
    {
        GORB.UpdateVelocity(type, dir);
    }

    public void ChangeGORBGravityDirection(GravityObjectRigidBody GORB, Vector2 dir)
    {
        GORB.ChangeGravityDirectionInternal(dir);
    }

    public void ClearVelocities(GameObject GORB)
    {
        GORB.GetComponent<GravityObjectRigidBody>().ClearAllVelocities();
    }

    public void InitializeForStartLevel(Vector3 spawnPoint, bool isDead)
    {
        transform.position = spawnPoint;
        GetComponent<GravityObjectRigidBody>().ClearAllVelocities();
        ChangeGORBGravityDirection(GetComponent<GravityObjectRigidBody>(), FindDirToClosestWall());
        IsDead = isDead;
    }

    protected void DoSpecial(Vector2 aimVector)
    {
        ControlledPlayer.SpecialsFired++;
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

    public void PlaySingle(AudioClip clip, int channel)
    {
        //Set the clip of our efxSource audio source to the clip passed in as a parameter.
        channels[channel].clip = clip;
        channels[channel].volume = 1.0f * AudioManager.SFXVol * AudioManager.MasterVol;
        //Play the clip.
        channels[channel].Play();
    }


    //RandomizeSfx chooses randomly between various audio clips
    public void RandomizeSfx(AudioClip[] clips, AudioClip[] caveClips, int channel)
    {
        //Generate a random number between 0 and the length of our array of clips passed in.
        int randomIndex;
        if (AudioManager.Instance.isCaveLevel)
            randomIndex = UnityEngine.Random.Range(0, caveClips.Length);
        else
            randomIndex = UnityEngine.Random.Range(0, clips.Length);

        //Choose a random pitch to play back our clip at between our high and low pitch ranges.
        float randomPitch = UnityEngine.Random.Range(AudioManager.Instance.lowPitchRange, AudioManager.Instance.highPitchRange);

        //Set the pitch of the audio source to the randomly chosen pitch.
        channels[channel].pitch = randomPitch;

        //Set the clip to the clip at our randomly chosen index.
        if (AudioManager.Instance.isCaveLevel)
            channels[channel].clip = caveClips[randomIndex];
        else
            channels[channel].clip = clips[randomIndex];
        channels[channel].volume = 1.0f * AudioManager.SFXVol * AudioManager.MasterVol;
        //Play the clip.
        channels[channel].Play();
    }
}