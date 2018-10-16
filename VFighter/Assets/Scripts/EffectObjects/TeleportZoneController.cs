using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportZoneController : MonoBehaviour {

    public TeleportZoneController TeleportTo;

    [SerializeField]
    private bool _waitingForCooldown;

    public List<GravityObjectRigidBody> ObjectsWaitingToExitTeleporter = new List<GravityObjectRigidBody>();

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!_waitingForCooldown)
        {
            Rigidbody2D rb = collision.attachedRigidbody;
            var gorb = collision.GetComponent<GravityObjectRigidBody>();
            if (gorb && !ObjectsWaitingToExitTeleporter.Contains(gorb))
            {
                TeleportTo.ObjectsWaitingToExitTeleporter.Add(gorb);

                var deltaPosition = collision.transform.position - transform.position;
                var teleporterScale = TeleportTo.transform.rotation * TeleportTo.transform.lossyScale;
                teleporterScale = new Vector3(Mathf.Abs(teleporterScale.x), Mathf.Abs(teleporterScale.y), Mathf.Abs(teleporterScale.z));
                
                collision.transform.position = TeleportTo.transform.position + Vector3.Project(deltaPosition, teleporterScale);
            }
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
