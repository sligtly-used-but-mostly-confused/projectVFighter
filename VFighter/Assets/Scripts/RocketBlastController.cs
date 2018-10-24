using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RocketBlastController : NetworkBehaviour {

    public float SecondsAlive = .5f;
    public float ExplosionVelocity = 20f;
	// Use this for initialization
	void Start () {
        StartCoroutine(DestroyOnTimer(SecondsAlive));
	}
	
	IEnumerator DestroyOnTimer(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        var GORB = collision.GetComponent<GravityObjectRigidBody>();
        if (GORB && isServer)
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
