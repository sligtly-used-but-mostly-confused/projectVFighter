using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Collider2D))]
public class GravityGunProjectileController : NetworkBehaviour {

    [SerializeField]
    private float _secondsUntilDestory = 1;

    public PlayerController Owner;

    public override void OnStartServer()
    {
        base.OnStartServer();
        StartCoroutine(Onstart());
    }
    
    IEnumerator Onstart () {
        yield return new WaitForSeconds(_secondsUntilDestory);
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
                    (gravityObjectRB as ControllableGravityObjectRigidBody).LastShotBy = Owner.ControlledPlayer;
                }
                Owner.AttachReticle(gravityObjectRB);
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
