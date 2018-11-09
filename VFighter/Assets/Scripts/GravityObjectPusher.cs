using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GravityObjectPusher : NetworkBehaviour {

    public Collider2D MagnetCollider;

    public float MagnetStrengthControllableTarget = 5;

    public void OnTriggerStay2D(Collider2D collision)
    {
        var gravityObjectRB = collision.GetComponent<GravityObjectRigidBody>();
        if (gravityObjectRB && isServer)
        {
            var dis = MagnetCollider.Distance(collision);
            var dir = dis.normal * Mathf.Abs(dis.distance);
            var forceVector = Vector3.zero;
            if (gravityObjectRB.GetComponent<ControllableGravityObjectRigidBody>())
            {
                var changeInGravDirection = dir.normalized - gravityObjectRB.GravityDirection;
                var newGravDirection = gravityObjectRB.GravityDirection + changeInGravDirection * MagnetStrengthControllableTarget * Time.deltaTime;

                gravityObjectRB.ChangeGravityDirectionInternal(newGravDirection.normalized);
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
                gravityObjectRB.AddVelocity(VelocityType.OtherPhysics, gravityObjectRB.GravityDirection * 10);
                gravityObjectRB.ChangeGravityDirectionInternal(Vector2.zero);

                //gravityObjectRB.ClearAllVelocities();
            }
        }
    }
}
