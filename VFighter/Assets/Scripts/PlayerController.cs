﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GravityObjectRigidBody))]
public abstract class PlayerController : MonoBehaviour {
    [SerializeField]
    protected float RechargeTime = .25f;
    [SerializeField]
    protected float MoveSpeed = 1f;
    [SerializeField]
    protected float JumpForce = 10f;

    [SerializeField]
    protected GameObject AimingReticle;

    public void Move(Vector3 dir)
    {
        GetComponent<GravityObjectRigidBody>().AddLinearAcceleration(dir * MoveSpeed);
    }

    public void AimReticle(Vector2 dir)
    {
        AimingReticle.transform.localPosition = dir.normalized;
    }

    public void ChangeGravity(Vector2 dir)
    {
        GetComponent<GravityObjectRigidBody>().ChangeGravityDirection(dir);
    }

    public void Jump()
    {
        throw new System.NotImplementedException();
    }

    public void ShootGravityGun(Vector3 dir)
    {
        throw new System.NotImplementedException();
    }
}
