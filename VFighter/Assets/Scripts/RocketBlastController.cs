﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RocketBlastController : NetworkBehaviour {

    public float SecondsOfExplosion = .5f;
    public float SecondsForEffect = 10f;
    public float ExplosionVelocity = 20f;

    public bool IsStillExploding = true;

	// Use this for initialization
	void Start () {
        StartCoroutine(DestroyOnTimer());
	}
	
	IEnumerator DestroyOnTimer()
    {
        yield return new WaitForSeconds(SecondsOfExplosion);
        IsStillExploding = false;
        yield return new WaitForSeconds(SecondsForEffect);
        Destroy(gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        var GORB = collision.GetComponent<GravityObjectRigidBody>();
        if (GORB && isServer && IsStillExploding)
        {
            var dir = GORB.transform.position - transform.position;
            GORB.UpdateVelocity(VelocityType.OtherPhysics, dir * ExplosionVelocity);
            if(collision.GetComponent<PlayerController>())
            {
                var compass = new List<Vector2>{ GORB.GravityDirection, -GORB.GravityDirection };
                GORB.ChangeGravityDirectionInternal(PlayerController.ClosestDirection(dir, compass.ToArray()));
            }
            else
            {
                GORB.ChangeGravityDirectionInternal(dir);
            }
        }
    }
}