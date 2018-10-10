using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ForceZoneController : MonoBehaviour
{
    public Vector2 gravityDirection = new Vector2(80f, -60f);
    public float Force = 10;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<GravityObjectRigidBody>())
        {
            collision.GetComponent<GravityObjectRigidBody>().AddVelocity(VelocityType.OtherPhysics, gravityDirection.normalized * Force);

            var players = FindObjectsOfType<PlayerController>().Where(x => x.isLocalPlayer);
            if(players.Count() > 0)
            {
                players.First().ChangeGORBGravityDirection(collision.GetComponent<GravityObjectRigidBody>(), gravityDirection.normalized);
            }
        }
    }

    public Vector2 GetgravityForce(){
        return gravityDirection;
    }
}
