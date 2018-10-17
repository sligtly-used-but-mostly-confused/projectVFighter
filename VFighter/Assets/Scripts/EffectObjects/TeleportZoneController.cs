using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportZoneController : MonoBehaviour {

    public TeleportZoneController TeleportTo;
    public List<GravityObjectRigidBody> ObjectsWaitingToExitTeleporter = new List<GravityObjectRigidBody>();

    [SerializeField]
    public Vector2 ForceKickDirection = Vector2.zero;
    [SerializeField]
    public float ForceKickScale = 100;

    [SerializeField]
    private float _objectExitFromTeleporterTimeout = .1f;

    private void OnTriggerStay2D(Collider2D collision)
    {
        Rigidbody2D rb = collision.attachedRigidbody;
        var gorb = collision.GetComponent<GravityObjectRigidBody>();
        if (gorb && !ObjectsWaitingToExitTeleporter.Contains(gorb))
        {
            Debug.Log("teleporting");
            TeleportTo.ObjectsWaitingToExitTeleporter.Add(gorb);

            //get the players offset from the teleporter so we can match it on the other side
            //also do a projection with the scale of the teleporter so that the player is garunteed to spawn 
            //within the teleporter when they come out
            var deltaPosition = collision.transform.position - transform.position;
            var teleporterScale = TeleportTo.transform.rotation * TeleportTo.transform.lossyScale;
            teleporterScale = new Vector3(Mathf.Abs(teleporterScale.x), Mathf.Abs(teleporterScale.y), Mathf.Abs(teleporterScale.z));
            var playerOffsetFromTeleporter = Vector3.Project(deltaPosition, teleporterScale);

            gorb.AddVelocity(VelocityType.OtherPhysics, TeleportTo.ForceKickDirection * TeleportTo.ForceKickScale);
            collision.transform.position = TeleportTo.transform.position + playerOffsetFromTeleporter;

            TeleportTo.StartCoroutine(TeleportTo.ObjectExitTeleporterTimeout(gorb));
        }
    }

    public IEnumerator ObjectExitTeleporterTimeout(GravityObjectRigidBody gorb)
    {
        yield return new WaitForSeconds(_objectExitFromTeleporterTimeout);

        //unity derped and didnt trigger the on trigger exit
        if (gorb && gorb.isActiveAndEnabled && ObjectsWaitingToExitTeleporter.Contains(gorb))
        {
            ObjectsWaitingToExitTeleporter.Remove(gorb);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        var gorb = collision.GetComponent<GravityObjectRigidBody>();
        if (gorb && ObjectsWaitingToExitTeleporter.Contains(gorb))
        {
            ObjectsWaitingToExitTeleporter.Remove(gorb);
        }
    }
}
