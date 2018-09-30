using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GravityGunProjectileController : MonoBehaviour {

    [SerializeField]
    private float _secondsUntilDestory = 1;

    public PlayerController Owner;

	// Use this for initialization
	IEnumerator Start () {
        yield return new WaitForSeconds(_secondsUntilDestory);
        Destroy(gameObject);
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var gravityObjectRB = collision.GetComponent<GravityObjectRigidBody>();
        if (gravityObjectRB)
        {
            if(collision.GetComponent<PlayerController>())
            {
                collision.GetComponent<PlayerController>().FlipGravity();
                Owner.IsCoolingDown = true;
                Owner.StartGravGunCoolDown();
                Owner.DestroyAllGravGunProjectiles();
                return;
            }

            if(gravityObjectRB.Owner != Owner && gravityObjectRB.CanBeSelected)
            {
                if(gravityObjectRB is ControllableGravityObjectRigidBody)
                {
                    (gravityObjectRB as ControllableGravityObjectRigidBody).StepMultiplier();
                    (gravityObjectRB as ControllableGravityObjectRigidBody).LastShotBy = Owner.ControlledPlayer;
                }

                gravityObjectRB.Owner = Owner;
                Owner.AttachGORB(gravityObjectRB);
                Owner.IsCoolingDown = true;
                Owner.StartGravGunCoolDown();
                Owner.DestroyAllGravGunProjectiles();
                return;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }
    }
}
