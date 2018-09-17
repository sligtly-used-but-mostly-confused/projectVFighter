using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(GravityObjectRigidBody))]

public abstract class PlayerController : MonoBehaviour {

    public List<GravityObjectRigidBody> AttachedObjects;
    
    [SerializeField]
    protected float RechargeTime = 1f;
    [SerializeField]
    protected float ChangeGravityRechargeTime = .1f;
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
    public bool IsDead;

    private readonly Vector2[] _compass = { Vector2.left, Vector2.right, Vector2.up, Vector2.down };

    private bool _isCoolingDown;
    private bool _isChangeGravityCoolingDown;

    protected virtual void Awake()
    {
        IsDead = false;
    }

    public void Move(Vector2 dir)
    {
        GetComponent<GravityObjectRigidBody>().AddLinearAcceleration(dir * MoveSpeed);
    }

    public void AimReticle(Vector2 dir)
    {
        AimingReticle.transform.localPosition = dir.normalized;
    }

    public void ChangeGravity(Vector2 dir)
    {
        /*
        _dataService.InsertAction(new PlayerAction(
            ActionType.ChangeGrav, 
            dir, 
            transform.position,
            GravityObjectManager.Instance.GetOtherPlayers(this), 
            GravityObjectManager.Instance.GravityObjectsNotPlayers));
            */
        if (!_isChangeGravityCoolingDown)
        {
            var closestDir = ClosestDirection(dir);
            GetComponent<GravityObjectRigidBody>().ChangeGravityDirection(closestDir);
            AttachedObjects.ForEach(x => x.ChangeGravityDirection(closestDir));
            StartCoroutine(ChangeGravityCoolDown());
        }
    }

    public void Jump()
    {
        throw new System.NotImplementedException();
    }

    public void ShootGravityGun(Vector2 dir)
    {
        if (!_isCoolingDown)
        {
            GameObject projectileClone = (GameObject)Instantiate(Projectile, AimingReticle.transform.position, AimingReticle.transform.rotation);
            projectileClone.GetComponent<GravityGunProjectileController>().Owner = this;
            projectileClone.GetComponent<Rigidbody2D>().velocity = dir * ShootSpeed;
            StartCoroutine(CoolDown());
        }
    }

    IEnumerator CoolDown()
    {
        _isCoolingDown = true;
        yield return new WaitForSeconds(RechargeTime);
        _isCoolingDown = false;
    }

    IEnumerator ChangeGravityCoolDown()
    {
        _isChangeGravityCoolingDown = true;
        yield return new WaitForSeconds(ChangeGravityRechargeTime);
        _isChangeGravityCoolingDown = false;
    }

    public void AttachGORB(GravityObjectRigidBody gravityObjectRB)
    {
        AttachedObjects.Add(gravityObjectRB);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var impulse = (collision.relativeVelocity * collision.rigidbody.mass).magnitude;

        //Debug.Log((impulse + " " + ImpulseToKill) + " " + collision.collider.GetComponent<GravityObjectRigidBody>());
        if (impulse > ImpulseToKill && collision.collider.GetComponent<GravityObjectRigidBody>())
        {
            if(collision.collider.GetComponent<PlayerController>())
            {
                Debug.Log("double kill");
            }
            Kill();
        }
    }

    public Vector2 ClosestDirection(Vector2 v)
    {

        var maxDot = -Mathf.Infinity;
        var ret = Vector3.zero;

        foreach (var dir in _compass)
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

    public void Kill()
    {
        Debug.Log("dead");
        IsDead = true;
    }
}
