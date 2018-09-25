using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportZoneController : MonoBehaviour {

    public TeleportZoneController TeleportTo;

    [SerializeField]
    private bool _waitingForCooldown;
    [SerializeField]
    private float _teleportCoolDownTime = .25f;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!_waitingForCooldown)
        {
            Rigidbody2D rb = collision.attachedRigidbody;

            if (collision.GetComponent<GravityObjectRigidBody>() || collision.GetComponent<GravityGunProjectileController>())
            {
                collision.transform.position = TeleportTo.transform.position;
                StartCoolDown();
                TeleportTo.StartCoolDown();
            }

        }
    }

    public void StartCoolDown()
    {
        StartCoroutine(CoolDown());
    }

    IEnumerator CoolDown()
    {
        _waitingForCooldown = true;
        yield return new WaitForSeconds(_teleportCoolDownTime);
        _waitingForCooldown = false;
    }
}
