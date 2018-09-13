using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(GravityObjectRigidBody))]
public abstract class PlayerController : MonoBehaviour {
    [SerializeField]
    protected float RechargeTime = .25f;
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

    public List<GravityObjectRigidBody> AttachedObjects;

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
        GetComponent<GravityObjectRigidBody>().ChangeGravityDirection(dir);
        AttachedObjects.ForEach(x => x.ChangeGravityDirection(dir));
    }

    public void Jump()
    {
        throw new System.NotImplementedException();
    }

    public void ShootGravityGun(Vector2 dir)
    {
        GameObject projectileClone = Instantiate(Projectile, AimingReticle.transform.position, AimingReticle.transform.rotation);
        projectileClone.GetComponent<GravityGunProjectileController>().Owner = this;
        projectileClone.GetComponent<Rigidbody2D>().velocity = dir * ShootSpeed;
    }

    public void AttachGORB(GravityObjectRigidBody gravityObjectRB)
    {
        AttachedObjects.Add(gravityObjectRB);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var impulse = (collision.relativeVelocity * collision.rigidbody.mass).magnitude;
        if (impulse > ImpulseToKill && collision.collider.GetComponent<GravityObjectRigidBody>())
        {
            Destroy(gameObject);
        }
    }
}
