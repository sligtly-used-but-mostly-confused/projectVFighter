using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalGravityGunProjectile : GravityGunProjectileController {

    public Collider2D MagnetCollider;

    private float MagnetStrengthNormal = -25;
    private float MagnetStrengthControllableTarget = 15;
    
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
                var changeInGravDirection = dir.normalized - GORB.GravityDirection;
                var newGravDirection = GORB.GravityDirection + changeInGravDirection * MagnetStrengthControllableTarget * Time.deltaTime;

                GORB.ChangeGravityDirectionInternal(newGravDirection.normalized);
            }
        }
    }
}
