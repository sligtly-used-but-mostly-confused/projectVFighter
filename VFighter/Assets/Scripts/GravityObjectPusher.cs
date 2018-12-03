using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GravityObjectPusher : MonoBehaviour
{

    public Collider2D MagnetCollider;

    public float MagnetStrengthControllableTarget = 10;

    public void OnTriggerStay2D(Collider2D collision)
    {
        var gravityObjectRB = collision.GetComponent<GravityObjectRigidBody>();
        if (gravityObjectRB)
        {
            var dis = MagnetCollider.Distance(collision);
            var dir = dis.normal * Mathf.Abs(dis.distance);
            var forceVector = Vector3.zero;
            if (gravityObjectRB.GetComponent<ControllableGravityObjectRigidBody>())
            {
                var changeInGravDirection = dir.normalized;
                var newGravDirection = gravityObjectRB.GravityDirection + changeInGravDirection * MagnetStrengthControllableTarget * Time.deltaTime;
                //Debug.Log(dir.normalized +" "+ gravityObjectRB.GravityDirection + " "+ changeInGravDirection * MagnetStrengthControllableTarget * Time.deltaTime + " "+changeInGravDirection + " "+newGravDirection);
                gravityObjectRB.ChangeGravityDirectionInternal(newGravDirection);
            }
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        var gravityObjectRB = collision.GetComponent<GravityObjectRigidBody>();
        if (gravityObjectRB)
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
