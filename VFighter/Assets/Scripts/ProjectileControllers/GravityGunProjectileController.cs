using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum ProjectileControllerType
{
    Normal,
    Shotgun,
    Rocket
}

[RequireComponent(typeof(Collider2D))]
public class GravityGunProjectileController : NetworkBehaviour {

    [SerializeField]
    public float SecondsUntilDestroy = 1;
    public PlayerController Owner;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public virtual void OnShot()
    {
        StartCoroutine(Onstart());
    }
    
    IEnumerator Onstart () {
        yield return new WaitForSeconds(SecondsUntilDestroy);
        ReturnToPool();
    }

    protected virtual void ReturnToPool()
    {
        ProjectilePool.Instance.ReturnProjectile(this, this.GetType());
    }

    public virtual void OnHitGORB(GravityObjectRigidBody GORB)
    {
        if (GORB.CanBeSelected)
        {
            if (GORB is ControllableGravityObjectRigidBody)
            {
                (GORB as ControllableGravityObjectRigidBody).StepMultiplier();
                (GORB as ControllableGravityObjectRigidBody).LastShotBy = Owner.netId;
            }

            Owner.AttachReticle(GORB);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var gravityObjectRB = collision.GetComponent<GravityObjectRigidBody>();
        if (gravityObjectRB && isServer)
        {
            if(collision.GetComponent<PlayerController>())
            {
                collision.GetComponent<PlayerController>().FlipGravity();
                ReturnToPool();
                return;
            }

            OnHitGORB(gravityObjectRB);
            ReturnToPool();
            return;
        }
    }
}
