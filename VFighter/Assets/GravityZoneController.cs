using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityZoneController : MonoBehaviour {

    public Vector2 gravityForce = new Vector2(80f, -60f);

    private void OnTriggerStay2D(Collider2D collision)
    {
        collision.attachedRigidbody.AddForce(transform.TransformDirection(gravityForce), ForceMode2D.Force);
        Debug.Log("adding force");
    }
}
