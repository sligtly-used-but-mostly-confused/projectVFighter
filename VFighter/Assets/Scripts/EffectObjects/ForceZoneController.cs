using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceZoneController : MonoBehaviour
{
    public Vector2 gravityForce = new Vector2(80f, -60f);
   
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<GravityObjectRigidBody>())
        {
            collision.GetComponent<GravityObjectRigidBody>().AddVelocity(VelocityType.OtherPhysics, gravityForce);
        }
    }
}
