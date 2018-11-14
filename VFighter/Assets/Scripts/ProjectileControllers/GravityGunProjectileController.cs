using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileControllerType
{
    Normal,
    Shotgun,
    Rocket
}

[RequireComponent(typeof(Collider2D))]
public class GravityGunProjectileController : MonoBehaviour
{

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
                (otherGORB as ControllableGravityObjectRigidBody).LastShotBy = Owner;
                var connectionToPlayer = otherGORB.GetComponent<ConnectionToPlayerController>();
                //if(connectionToPlayer)
                {
                    connectionToPlayer.ConnectToPlayer(Owner);
                }
            }

            Owner.AttachReticle(otherGORB);
        }
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        var gravityObjectRB = collision.GetComponent<GravityObjectRigidBody>();
        //we have 2 colliders on some objects we only want this on tigger to deal with the main collider so check if it was actually the one that collided
        Collider2D[] res = new Collider2D[100];

        var numRes = collision.GetContacts(res);
        bool isProjectileBodyFound = false;

        for (int i = 0; i < numRes; i++)
        {
            isProjectileBodyFound |= res[i].tag == "ProjectileBody";
        }

        if(!isProjectileBodyFound)
        {
            return;
        }

        if (gravityObjectRB)
        {

            if (collision.GetComponent<PlayerController>() && collision.GetComponent<PlayerController>() != Owner)
            {
                collision.GetComponent<PlayerController>().FlipGravity();
                ReturnToPool();
                return;
            }

            if (collision.GetComponent<PlayerController>() && collision.GetComponent<PlayerController>() == Owner)
            {
                return;
            }

            OnHitGORB(gravityObjectRB);
            ReturnToPool();
            return;
        }
    }
}
