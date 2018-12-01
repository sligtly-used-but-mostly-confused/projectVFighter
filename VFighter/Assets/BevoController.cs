using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BevoController : MonoBehaviour {
    public TutorialEvent Event;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<ControllableGravityObjectRigidBody>())
        {
            Event.OnFinish.Invoke();
        }
    }
}
