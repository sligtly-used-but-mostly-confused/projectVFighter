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

    protected GravityObjectRigidBody GORB;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        GORB = GetComponent<GravityObjectRigidBody>();
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

    public virtual void OnHitGORB(GravityObjectRigidBody otherGORB)
    {
        if (otherGORB.CanBeSelected)
        {
            if (otherGORB is ControllableGravityObjectRigidBody)
            {
                (otherGORB as ControllableGravityObjectRigidBody).StepMultiplier();
                (otherGORB as ControllableGravityObjectRigidBody).LastShotBy = Owner.netId;
                var connectionToPlayer = otherGORB.GetComponent<ConnectionToPlayerController>();
                //if(connectionToPlayer)
                {
                    connectionToPlayer.ConnectToPlayer(Owner);
                }
            }

            Owner.AttachReticle(otherGORB);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var gravityObjectRB = collision.GetComponent<GravityObjectRigidBody>();
        if (gravityObjectRB && isServer)
        {
            if(collision.GetComponent<PlayerController>() && collision.GetComponent<PlayerController>() != Owner)
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
