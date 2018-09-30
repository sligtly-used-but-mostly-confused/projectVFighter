using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllableGravityObjectRigidBody : GravityObjectRigidBody {
    [SerializeField]
    private float _speedMultiplier;
    [SerializeField]
    private float _speedMultiplierStep;
    [SerializeField]
    private float _maxSpeedMultiplier;

    public Player LastShotBy;

    public void StepMultiplier()
    {
        _speedMultiplier *= 1 + _speedMultiplierStep;
        _speedMultiplier = Mathf.Clamp(_speedMultiplier, 1, _maxSpeedMultiplier);
    }

    public override void UpdateVelocity(VelocityType id, Vector2 vel)
    {
        if (!_velocities.ContainsKey(id))
        {
            _velocities.Add(id, vel);
        }

        _velocities[id] = vel;
    }

    public override Vector2 GetMaxComponentVelocity(VelocityType type)
    {
        return base.GetMaxComponentVelocity(type) * _speedMultiplier;
    }

    
    public override void AddVelocity(VelocityType id, Vector2 velocityVector)
    {
        base.AddVelocity(id, velocityVector * _speedMultiplier);
    }
    
}
