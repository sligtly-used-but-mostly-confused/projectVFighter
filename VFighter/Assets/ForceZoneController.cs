using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceZoneController : MonoBehaviour
{
    public GameObject AffectedPrefab;
    public Vector2 gravityForce = new Vector2(80f, -60f);
   
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<GravityObjectRigidBody>() || 
            collision.GetComponent<GravityGunProjectileController>())
        {
            collision.attachedRigidbody.AddForce(transform.TransformDirection(gravityForce), ForceMode2D.Force);
        }
    }
}
