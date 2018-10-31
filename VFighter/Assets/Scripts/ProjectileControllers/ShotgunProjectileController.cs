using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunProjectileController : GravityGunProjectileController
{
    public override void OnHitGORB(GravityObjectRigidBody GORB)
    {
        if (GORB.CanBeSelected)
        {
            if (GORB is ControllableGravityObjectRigidBody)
            {
                (GORB as ControllableGravityObjectRigidBody).StepMultiplier();
                (GORB as ControllableGravityObjectRigidBody).LastShotBy = Owner.netId;
            }

            GORB.ChangeGravityDirectionInternal(GetComponent<GravityObjectRigidBody>().GetVelocity(VelocityType.Gravity));
        }
    }
}
