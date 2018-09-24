﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(GravityObjectRigidBody))]

public abstract class PlayerController : MonoBehaviour {

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
    protected GameObject AimingReticle;
    [SerializeField]
    protected float DashSpeed = 10f;
    [SerializeField]
    protected Player ControlledPlayer;
    

    protected readonly Vector2[] _gravChangeDirections = {Vector2.up, Vector2.down };
    protected readonly Vector2[] _gravChangeDirectionsForThrownObject = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    public bool IsCoolingDown = false;
    public bool IsChangeGravityCoolingDown = false;
    public bool IsDashCoolingDown = false;
    public bool IsDead;

    private List<GameObject> GravityGunProjectiles = new List<GameObject>();

    private Coroutine GravGunCoolDownCoroutine;

    public virtual void Init(Player player, SpawnPosition spawnPosition)
    {
        GetComponent<GravityObjectRigidBody>().ChangeGravityDirection(Vector2.zero);
        ControlledPlayer = player;
        transform.position = spawnPosition.transform.position;
        GetComponent<Renderer>().material = ControlledPlayer.PlayerMaterial;
        AimingReticle.GetComponent<Renderer>().material = ControlledPlayer.PlayerMaterial;
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
        if (!IsChangeGravityCoolingDown)
        {
            ChangeGravity(GetComponent<GravityObjectRigidBody>().GravityDirection * -1);
        }
    }

    public void ChangeGravity(Vector2 dir)
    {
        if (!IsChangeGravityCoolingDown)
        {
            var closestDir = ClosestDirection(dir, _gravChangeDirections);
            GetComponent<GravityObjectRigidBody>().ChangeGravityDirection(closestDir);
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
        if(!IsDashCoolingDown)
        {
            //need to account for gravity
            var dashVec = -GetComponent<GravityObjectRigidBody>().GravityDirection.normalized * DashSpeed + dir * DashSpeed;
            GetComponent<GravityObjectRigidBody>().UpdateVelocity(VelocityType.Dash, dashVec);

            IsDashCoolingDown = true;
            StartCoroutine(DashCoolDown());
        }
        
    } 

    public void AimReticle(Vector2 dir)
    {
        var aimParent = AimingReticle.transform.parent;
        var normalizedDir = dir.normalized;
        AimingReticle.transform.position = aimParent.position + new Vector3(normalizedDir.x, normalizedDir.y, 0);
        //AimingReticle.transform.localPosition = dir.normalized * 2;
    }

    public void ShootGravityGun(Vector2 dir)
    {
        dir = dir.normalized;
        if (!IsCoolingDown)
        {
            if(AttachedObject == null)
            {
                GameObject projectileClone = (GameObject)Instantiate(Projectile, AimingReticle.transform.position, AimingReticle.transform.rotation);
                projectileClone.GetComponent<GravityGunProjectileController>().Owner = this;
                projectileClone.GetComponent<Rigidbody2D>().velocity = dir * ShootSpeed;
                projectileClone.GetComponent<Renderer>().material = ControlledPlayer.PlayerMaterial;
                StartGravGunCoolDown();
                GravityGunProjectiles.Add(projectileClone);
            }
            else
            { 
                AttachedObject.ChangeGravityDirection(dir);
                DetachGORB();
            }
        }
    }

    public void AttachGORB(GravityObjectRigidBody gravityObjectRB)
    {
        AttachedObject = gravityObjectRB;
        AimingReticle.transform.parent = AttachedObject.transform;
    }

    public void DetachGORB()
    {
        AttachedObject.Owner = null;
        AttachedObject = null;
        AimingReticle.transform.parent = transform;
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
        var impulse = (collision.relativeVelocity * collision.rigidbody.mass).magnitude;
        var GORB = collision.collider.GetComponent<GravityObjectRigidBody>();
        if (impulse > ImpulseToKill && GORB && GORB.KillsPlayer)
        {
            if(collision.collider.GetComponent<PlayerController>())
            {
                Debug.Log("double kill");
                if(IsDashCoolingDown)
                {
                    //dont kil because we dashed into this
                    return;
                }
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
    }

    public void DestroyAllGravGunProjectiles()
    {
        GravityGunProjectiles.ForEach(x => Destroy(x));
        GravityGunProjectiles.Clear();
    }
}
