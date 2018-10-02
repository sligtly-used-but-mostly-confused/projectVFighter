using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System;

[RequireComponent(typeof(GravityObjectRigidBody))]

public abstract class PlayerController : NetworkBehaviour {
    private static short _aimingReticleIdCnt = 0;

    public GravityObjectRigidBody AttachedObject;

    [SerializeField]
    protected float RechargeTime = 1f;
    [SerializeField]
    protected float ChangeGravityRechargeTime = .1f;
    [SerializeField]
    protected float DashCoolDownTime = .1f;
    [SerializeField]
    protected float MoveSpeed = 1f;
    [SerializeField]
    protected float ShootSpeed = 1f;
    [SerializeField]
    protected float JumpForce = 10f;
    [SerializeField]
    protected float ImpulseToKill = 10f;
    [SerializeField]
    protected GameObject Projectile;

    [SerializeField]
    protected float DashSpeed = 10f;

    public GameObject AimingReticlePrefab;
    [SerializeField]
    protected GameObject Reticle;
    [SerializeField]
    protected GameObject ReticleParent;

    protected readonly Vector2[] _gravChangeDirections = { Vector2.up, Vector2.down };
    protected readonly Vector2[] _gravChangeDirectionsForThrownObject = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    

    public bool IsCoolingDown = false;
    public bool IsChangeGravityCoolingDown = false;
    public bool IsDashCoolingDown = false;
    public bool IsDead;

    private List<GameObject> GravityGunProjectiles = new List<GameObject>();
    private Coroutine GravGunCoolDownCoroutine;
    [SerializeField]
    protected InputDevice InputDevice;

    [SyncVar]
    public short ReticleId = -1;
    [SyncVar]
    public short MaterialId = -1;
    [SyncVar]
    public bool IsReady = false;
    [SyncVar]
    public Player ControlledPlayer;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(FindReticle());
    }

    private void Update()
    {
        FindReticle();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        GameObject aimingReticle = Instantiate(AimingReticlePrefab);
        aimingReticle.GetComponent<Renderer>().material = GetComponent<Renderer>().material;
        _aimingReticleIdCnt++;
        aimingReticle.GetComponent<AimingReticle>().Id = _aimingReticleIdCnt;
        ReticleId = _aimingReticleIdCnt;

        GetComponent<Renderer>().material = CustomNetworkManager.Instance._playerMaterials[MaterialId];
        aimingReticle.GetComponent<Renderer>().material = CustomNetworkManager.Instance._playerMaterials[MaterialId];

        NetworkServer.SpawnWithClientAuthority(aimingReticle, connectionToClient);
        ReticleParent = gameObject;

        ControlledPlayer.NumLives = ControllerSelectManager.Instance.numLivesPerPlayer;
    }

    public override void OnStartLocalPlayer()
    {
        StartCoroutine(AttachInputDeviceToPlayer());
        
        GetComponent<Renderer>().material = CustomNetworkManager.Instance._playerMaterials[MaterialId];
    }

    IEnumerator AttachInputDeviceToPlayer()
    {
        GetComponent<GravityObjectRigidBody>().IsSimulatedOnThisConnection = isLocalPlayer;

        if (!isLocalPlayer)
        {

        }
        else if (ControllerSelectManager.Instance.DevicesWaitingForPlayer.Count > 0)
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

    public virtual void Init(Player player, Transform spawnPosition)
    {
        ControlledPlayer = player;
        transform.position = spawnPosition.transform.position;
        //GetComponent<Renderer>().material = ControlledPlayer.PlayerMaterial;
        //AimingReticle.GetComponent<Renderer>().material = ControlledPlayer.PlayerMaterial;
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

    protected virtual void Awake()
    {
        IsCoolingDown = false;
        IsChangeGravityCoolingDown = false;
        IsDashCoolingDown = false;
        IsDead = false;
    }

    public void Move(float dir)
    {
        GetComponent<GravityObjectRigidBody>().UpdateVelocity(VelocityType.Movement, new Vector2(dir * MoveSpeed, 0));
    }

    public void FlipGravity()
    {
        if (GetComponent<GravityObjectRigidBody>().IsSimulatedOnThisConnection)
        {
            if (!IsChangeGravityCoolingDown)
            {
                ChangeGravity(GetComponent<GravityObjectRigidBody>().GravityDirection * -1);
            }
        }
        else
        {
            CmdBroadcastFlipGravity();
        }
    }

    [Command]
    public void CmdBroadcastFlipGravity()
    {
        Debug.Log("cmd flip");
        RpcBroadcastFilpGravity();
    }

    [ClientRpc]
    public void RpcBroadcastFilpGravity()
    {
        Debug.Log("rpc flip");
        if (GetComponent<GravityObjectRigidBody>().IsSimulatedOnThisConnection)
        {
            FlipGravity();
        }
    }

    public void ChangeGravity(Vector2 dir)
    {
        if (!IsChangeGravityCoolingDown)
        {
            var closestDir = ClosestDirection(dir, _gravChangeDirections);
            ChangeGORBGravityDirection(GetComponent<GravityObjectRigidBody>(), closestDir);
            IsChangeGravityCoolingDown = true;
            StartCoroutine(ChangeGravityCoolDown());
        }
    }

    public void Jump()
    {
        throw new System.NotImplementedException();
    }

    public void Dash(Vector2 dir)
    {
        if (!IsDashCoolingDown)
        {
            //need to account for gravity
            var dashVec = dir.normalized * DashSpeed;
            GetComponent<GravityObjectRigidBody>().Dash(dashVec);

            IsDashCoolingDown = true;
            StartCoroutine(DashCoolDown());
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
                Reticle.GetComponent<Renderer>().material = CustomNetworkManager.Instance._playerMaterials[MaterialId];
                GetComponent<Renderer>().material = CustomNetworkManager.Instance._playerMaterials[MaterialId];
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
        if (GetComponent<GravityObjectRigidBody>().IsSimulatedOnThisConnection)
        {
            if (Reticle)
            {
                //var aimParent = AimingReticle.transform.parent;

                var normalizedDir = dir.normalized;
                Reticle.transform.position = ReticleParent.transform.position + new Vector3(normalizedDir.x, normalizedDir.y, 0);
            }
        }


    }

    public void ShootGravityGun(Vector2 dir)
    {
        if (GetComponent<GravityObjectRigidBody>().IsSimulatedOnThisConnection)
        {
            dir = dir.normalized;
            if (!IsCoolingDown)
            {
                if (AttachedObject == null)
                {
                    CmdSpawnProjectile(dir);
                    //projectileClone.GetComponent<Renderer>().material = ControlledPlayer.PlayerMaterial;
                    StartGravGunCoolDown();
                    //GravityGunProjectiles.Add(projectileClone);
                }
                else
                {
                    //AttachedObject.ChangeGravityDirection(dir);
                    ChangeGORBGravityDirection(AttachedObject, dir);
                    DetachReticle();
                }
            }
        }
    }

    [Command]
    public void CmdSpawnProjectile(Vector2 dir)
    {
        GameObject projectileClone = Instantiate(Projectile, Reticle.transform.position, Reticle.transform.rotation);
        projectileClone.GetComponent<GravityGunProjectileController>().Owner = this;
        projectileClone.GetComponent<GravityObjectRigidBody>().UpdateVelocity(VelocityType.OtherPhysics, dir * ShootSpeed);
        NetworkServer.Spawn(projectileClone);
    }

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

    public void DetachReticle()
    {
        AttachedObject.Owner = null;
        AttachedObject = null;
        ReticleParent = gameObject;
    }

    public void StartGravGunCoolDown()
    {
        if (GravGunCoolDownCoroutine == null)
        {
            GravGunCoolDownCoroutine = StartCoroutine(GravGunCoolDown());
        }
    }

    public IEnumerator GravGunCoolDown()
    {
        IsCoolingDown = true;
        yield return new WaitForSeconds(RechargeTime);
        IsCoolingDown = false;
        GravGunCoolDownCoroutine = null;
    }

    IEnumerator ChangeGravityCoolDown()
    {
        yield return new WaitForSeconds(ChangeGravityRechargeTime);
        IsChangeGravityCoolingDown = false;
    }

    IEnumerator DashCoolDown()
    {
        IsDashCoolingDown = true;
        yield return new WaitForSeconds(DashCoolDownTime);
        IsDashCoolingDown = false;
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
        if (impulse > ImpulseToKill && GORB && GORB.KillsPlayer)
        {
            if(collision.collider.GetComponent<PlayerController>())
            {
                if(IsDashCoolingDown)
                {
                    ControlledPlayer.NumKills++;
                    //kill the other player
                    collision.collider.GetComponent<PlayerController>().Kill();
                    return;
                }

                //dont kill if we run into another player
                return;
            }

            Debug.Log("player killed " + GORB.Owner);

            if(GORB is ControllableGravityObjectRigidBody && (GORB as ControllableGravityObjectRigidBody).LastShotBy.NetworkControllerId != 0)
            {
                (GORB as ControllableGravityObjectRigidBody).LastShotBy.NumKills++;
            }

            Kill();
        }
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

    public virtual void Kill()
    {
        IsDead = true;
        ControlledPlayer.NumDeaths++;
        gameObject.SetActive(!LevelManager.Instance.PlayersCanDieInThisLevel);
    }

    public void DestroyAllGravGunProjectiles()
    {
        GravityGunProjectiles.ForEach(x => Destroy(x));
        GravityGunProjectiles.Clear();
    }

    public void Ready()
    {
        IsReady = true;
    }

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
        Debug.Log("cmd change grav");
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
        Debug.Log("rpc change grav");
        if (GORB.GetComponent<GravityObjectRigidBody>().IsSimulatedOnThisConnection)
        {
            GORB.GetComponent<GravityObjectRigidBody>().ChangeGravityDirectionInternal(dir);
        }
    }

    public void InitializeForStartLevel(GameObject spawnPoint)
    {
        //
    }
}