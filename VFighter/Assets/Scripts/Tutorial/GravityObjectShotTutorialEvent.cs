using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityObjectShotTutorialEvent : TutorialEvent
{
    public ControllableGravityObjectRigidBody AttachedGORB;

    private void Start()
    {
        AttachedGORB.OnShot += (x, y) => { OnFinish.Invoke(); };
    }
}
