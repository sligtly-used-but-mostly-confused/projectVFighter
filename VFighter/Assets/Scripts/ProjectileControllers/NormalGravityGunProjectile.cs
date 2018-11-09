using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalGravityGunProjectile : GravityGunProjectileController {

    public Collider2D MagnetCollider;
    
    private float MagnetStrengthControllableTarget = 15;
    
    public void OnTriggerStay2D(Collider2D collision)
    {
        var gravityObjectRB = collision.GetComponent<GravityObjectRigidBody>();
        if (gravityObjectRB && isServer)
        {
            if(gravityObjectRB.GetComponent<ControllableGravityObjectRigidBody>())
            {
                var dis = MagnetCollider.Distance(collision);
                var dir = dis.normal * Mathf.Abs(dis.distance);
                var forceVector = Vector3.zero;
                var changeInGravDirection = dir.normalized - GORB.GravityDirection;
                var newGravDirection = GORB.GravityDirection + changeInGravDirection * MagnetStrengthControllableTarget * Time.deltaTime;

                GORB.ChangeGravityDirectionInternal(newGravDirection.normalized);
            }
        }
    }
}
