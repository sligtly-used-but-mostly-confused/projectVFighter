using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalGravityGunProjectile : GravityGunProjectileController {

    public Collider2D MagnetCollider;

    private float MagnetStrengthNormal = -25;
    private float MagnetStrengthControllableTarget = 50;
    
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        var gravityObjectRB = collision.GetComponent<GravityObjectRigidBody>();
        if (gravityObjectRB && isServer)
        {
            if (gravityObjectRB.GetComponent<ControllableGravityObjectRigidBody>())
            {
                GORB.ChangeGravityScale(.1f);
                GORB.UpdateVelocity(VelocityType.Gravity, Vector3.zero);
            }
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        var gravityObjectRB = collision.GetComponent<GravityObjectRigidBody>();
        if (gravityObjectRB && isServer)
        {
            if (gravityObjectRB.GetComponent<ControllableGravityObjectRigidBody>())
            {
                GORB.ChangeGravityScale(1);
                GORB.UpdateVelocity(VelocityType.Gravity, Vector3.zero);
            }
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        var gravityObjectRB = collision.GetComponent<GravityObjectRigidBody>();
        if (gravityObjectRB && isServer)
        {
            var dis = MagnetCollider.Distance(collision);
            var dir = dis.normal * Mathf.Abs(dis.distance);
            var forceVector = Vector3.zero;
            if(gravityObjectRB.GetComponent<ControllableGravityObjectRigidBody>())
            {
                forceVector = (dir / (dis.distance * dis.distance)) * MagnetStrengthControllableTarget;
                GORB.UpdateVelocity(VelocityType.Gravity, Vector3.zero);
            }

            forceVector *= Time.fixedDeltaTime;

            GORB.AddVelocity(VelocityType.OtherPhysics, forceVector);
        }
    }
}
