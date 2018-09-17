using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GravityGunProjectileController : MonoBehaviour {

    [SerializeField]
    private float _secondsUntilDestory = 1;

    public PlayerController Owner;

	// Use this for initialization
	IEnumerator Start () {
        yield return new WaitForSeconds(_secondsUntilDestory);
        Destroy(gameObject);
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var gravityObjectRB = collision.GetComponent<GravityObjectRigidBody>();
        if (gravityObjectRB)
        {
            if(gravityObjectRB.Owner != Owner)
            {
                gravityObjectRB.Owner = Owner;
                Owner.AttachGORB(gravityObjectRB);
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("same owner");
                gravityObjectRB.Owner = null;
                Owner.DetachGORB(gravityObjectRB);
                Destroy(gameObject);
            }
        }
    }
}
