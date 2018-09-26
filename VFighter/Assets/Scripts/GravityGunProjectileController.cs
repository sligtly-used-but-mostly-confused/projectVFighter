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
                gravityObjectRB.ChangeGravityDirection(Vector2.zero);
                Owner.IsCoolingDown = true;
                Owner.StartGravGunCoolDown();
                Owner.DestroyAllGravGunProjectiles();
                return;
            }

            if(gravityObjectRB.Owner != Owner && gravityObjectRB.CanBeSelected)
            {
                var condPlat = gravityObjectRB.GetComponentInParent<ConductivePlatformController>();
                if (condPlat){

                    condPlat.charge();
                }

                if(gravityObjectRB is ControllableGravityObjectRigidBody)
                {
                    (gravityObjectRB as ControllableGravityObjectRigidBody).StepMultiplier();
                    gravityObjectRB.Owner = Owner;
                    Owner.AttachGORB(gravityObjectRB);
                }

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
