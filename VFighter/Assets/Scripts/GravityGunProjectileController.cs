using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Collider2D))]
public class GravityGunProjectileController : NetworkBehaviour {

    [SerializeField]
    public float SecondsUntilDestroy = 1;
    public bool IsShotgunProjectile = false;
    public PlayerController Owner;

    public override void OnStartServer()
    {
        base.OnStartServer();
        StartCoroutine(Onstart());
    }
    
    IEnumerator Onstart () {
        yield return new WaitForSeconds(SecondsUntilDestroy);
        NetworkServer.Destroy(gameObject);
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var gravityObjectRB = collision.GetComponent<GravityObjectRigidBody>();
        if (gravityObjectRB && isServer)
        {
            if(collision.GetComponent<PlayerController>())
            {
                collision.GetComponent<PlayerController>().FlipGravity();
                Owner.IsCoolingDown = true;
                Owner.StartGravGunCoolDown();
                NetworkServer.Destroy(gameObject);
                return;
            }

            if(gravityObjectRB.CanBeSelected)
            {
                if(gravityObjectRB is ControllableGravityObjectRigidBody)
                {
                    (gravityObjectRB as ControllableGravityObjectRigidBody).StepMultiplier();
                    (gravityObjectRB as ControllableGravityObjectRigidBody).LastShotBy = Owner.netId;
                }

                if(IsShotgunProjectile)
                {
                    Debug.Log(GetComponent<GravityObjectRigidBody>().GetVelocity(VelocityType.OtherPhysics));
                    gravityObjectRB.ChangeGravityDirectionInternal(GetComponent<GravityObjectRigidBody>().GetVelocity(VelocityType.OtherPhysics));
                    //Owner.ChangeGORBGravityDirection(gravityObjectRB, GetComponent<GravityObjectRigidBody>().GravityDirection);
                }
                else
                {
                    Owner.AttachReticle(gravityObjectRB);
                }
                
                Owner.IsCoolingDown = true;
                Owner.StartGravGunCoolDown();
                NetworkServer.Destroy(gameObject);
                return;
            }
            else
            {
                NetworkServer.Destroy(gameObject);
                return;
            }
        }
    }
}
