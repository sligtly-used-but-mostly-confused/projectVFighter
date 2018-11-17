using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetZoneController : MonoBehaviour {

    public GravityObjectRigidBody GORB;

    private float MagnetStrengthControllableTarget = 15;

    private List<ControllableGravityObjectRigidBody> ObjectsInsideOfMagnetZone = new List<ControllableGravityObjectRigidBody>();
    private ControllableGravityObjectRigidBody _currentlyTracking;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        var gravityObjectRB = collision.GetComponent<ControllableGravityObjectRigidBody>();
        if (gravityObjectRB)
        {
            ObjectsInsideOfMagnetZone.Add(gravityObjectRB);
            if(_currentlyTracking == null)
            {
                _currentlyTracking = gravityObjectRB;
            }
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        var gravityObjectRB = collision.GetComponent<ControllableGravityObjectRigidBody>();
        if (gravityObjectRB)
        {
            ObjectsInsideOfMagnetZone.Remove(gravityObjectRB);
            if(gravityObjectRB == _currentlyTracking)
            {
                _currentlyTracking = null;
            }
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        var gravityObjectRB = collision.GetComponent<ControllableGravityObjectRigidBody>();

        if (gravityObjectRB && gravityObjectRB == _currentlyTracking)
        {
            Collider2D MagnetCollider = GetComponent<Collider2D>();
            var dis = MagnetCollider.Distance(collision);
            var dir = dis.normal * Mathf.Abs(dis.distance);
            var forceVector = Vector3.zero;
            var changeInGravDirection = dir.normalized - GORB.GravityDirection;
            var newGravDirection = GORB.GravityDirection + changeInGravDirection * MagnetStrengthControllableTarget * Time.deltaTime;

            GORB.ChangeGravityDirectionInternal(newGravDirection.normalized);
        }
        
    }
}
