﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportZoneController : MonoBehaviour {

    public GameObject TeleportTo;

    private void OnTriggerStay2D(Collider2D collision)
    {
        Rigidbody2D rb = collision.attachedRigidbody;

        if (collision.GetComponent<GravityObjectRigidBody>() || collision.GetComponent<GravityGunProjectileController>())
        {
            collision.transform.position = TeleportTo.transform.position;
        }
    }
}