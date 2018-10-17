using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ForceZoneController : MonoBehaviour
{
    public Vector2 gravityDirection = new Vector2(1, 1);
    public float Force = 4f;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<GravityObjectRigidBody>() && collision.GetComponent<GravityObjectRigidBody>().IsSimulatedOnThisConnection)
        {
            var changeInGravDirection = gravityDirection.normalized - collision.GetComponent<GravityObjectRigidBody>().GravityDirection;
            var newGravDirection = collision.GetComponent<GravityObjectRigidBody>().GravityDirection + changeInGravDirection * Force * Time.deltaTime;
            var players = FindObjectsOfType<PlayerController>().Where(x => x.isLocalPlayer);
            if (players.Count() > 0)
            {
                players.First().ChangeGORBGravityDirection(collision.GetComponent<GravityObjectRigidBody>(), newGravDirection);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var players = FindObjectsOfType<PlayerController>().Where(x => x.isLocalPlayer);
        if (players.Count() > 0 && collision.GetComponent<GravityObjectRigidBody>().IsSimulatedOnThisConnection)
        {
            players.First().ChangeGORBGravityDirection(collision.GetComponent<GravityObjectRigidBody>(), gravityDirection.normalized);
        }
    }

    public Vector2 GetgravityForce(){
        return gravityDirection;
    }
}
