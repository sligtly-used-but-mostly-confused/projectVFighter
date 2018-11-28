using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunProjectileController : GravityGunProjectileController
{

    public float PlayerKnockBackForce = 2.5f;

    public override void OnHitPlayer(PlayerController player)
    {
        var dir = GetComponent<GravityObjectRigidBody>().GetVelocity(VelocityType.Gravity);

        var GORB = player.GetComponent<GravityObjectRigidBody>();
        GORB.ClearAllVelocities();
        GORB.ChangeGravityScale(0);
        GORB.UpdateVelocity(VelocityType.Dash, dir * PlayerKnockBackForce);
        player.GetComponent<PlayerCooldownController>().StartCooldown(CooldownType.ShotgunKnockback, () =>
        {
            GORB.ClearAllVelocities();
            GORB.ChangeGravityScale(1);
        });

        
    }

    public override void OnHitGORB(GravityObjectRigidBody GORB)
    {
        if (GORB.CanBeSelected)
        {
            if (GORB is ControllableGravityObjectRigidBody)
            {
                (GORB as ControllableGravityObjectRigidBody).StepMultiplier();
                (GORB as ControllableGravityObjectRigidBody).LastShotBy = Owner;
            }

            GORB.ChangeGravityDirectionInternal(GetComponent<GravityObjectRigidBody>().GetVelocity(VelocityType.Gravity));
        }
    }
}
