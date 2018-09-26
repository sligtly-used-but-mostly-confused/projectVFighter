using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedZoneController : MonoBehaviour {

    public float SpeedIncreaseForce = 40f;


    private void OnTriggerStay2D(Collider2D collision)
    {
        Rigidbody2D rb = collision.attachedRigidbody;

        if(collision.GetComponent<GravityObjectRigidBody>() || collision.GetComponent<GravityGunProjectileController>())
        {
            collision.attachedRigidbody.AddForce(transform.TransformDirection(rb.velocity * SpeedIncreaseForce), ForceMode2D.Force);
        }
    }
}
