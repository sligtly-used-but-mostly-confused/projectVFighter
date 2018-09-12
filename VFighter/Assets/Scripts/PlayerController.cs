using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    protected Rigidbody2D Projectile;

    [SerializeField]
    protected GameObject AimingReticle;

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
    }

    public void Jump()
    {
        throw new System.NotImplementedException();
    }

    public void ShootGravityGun(Vector2 dir)
    {
        Debug.Log("Im trying!");
        Rigidbody2D projectileClone = (Rigidbody2D)Instantiate(Projectile, AimingReticle.transform.position, AimingReticle.transform.rotation);
        projectileClone.velocity = dir * ShootSpeed;
    }
}
