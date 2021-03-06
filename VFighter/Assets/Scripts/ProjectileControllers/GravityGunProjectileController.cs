﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileControllerType
{
    Normal,
    Shotgun,
    Rocket
}

[RequireComponent(typeof(Collider2D))]
public class GravityGunProjectileController : MonoBehaviour
{

    [SerializeField]
    public float SecondsUntilDestroy = 1;
    public PlayerController Owner;

    protected GravityObjectRigidBody GORB;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        GORB = GetComponent<GravityObjectRigidBody>();
    }

    public virtual void OnShot()
    {
        StartCoroutine(Onstart());
    }

    IEnumerator Onstart () {
        float timeLeft = SecondsUntilDestroy;
        while (timeLeft > 0)
        {
            yield return new WaitForEndOfFrame();
            timeLeft -= Time.deltaTime * GameManager.Instance.TimeScale;
        }
        
        ReturnToPool();
    }

    protected virtual void ReturnToPool()
    {
        ProjectilePool.Instance.ReturnProjectile(this, this.GetType());
    }

    public virtual void OnHitGORB(GravityObjectRigidBody otherGORB)
    {
        if (otherGORB.CanBeSelected)
        {
            if (otherGORB is ControllableGravityObjectRigidBody)
            {
                var CGORB = (otherGORB as ControllableGravityObjectRigidBody);
                CGORB.StepMultiplier();
                CGORB.AttachedPlayer = Owner;
                CGORB.LastShotBy = Owner;
            }

            Owner.AttachReticle(otherGORB);
        }
    }

    public virtual void OnHitPlayer(PlayerController player)
    {
        player.GetComponent<PlayerController>().FlipGravity();
    }

    protected bool TryGetCollisionContactWithTag(Collider2D collision, string tag)
    {
        var gravityObjectRB = collision.GetComponent<GravityObjectRigidBody>();
        Collider2D[] res = new Collider2D[100];

        var numRes = collision.GetContacts(res);
        bool isProjectileBodyFound = false;

        for (int i = 0; i < numRes; i++)
        {
            isProjectileBodyFound |= res[i].tag == tag;
        }

        return isProjectileBodyFound;
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        var gravityObjectRB = collision.GetComponent<GravityObjectRigidBody>();

        if (!TryGetCollisionContactWithTag(collision, "ProjectileBody"))
        {
            return;
        }
        
        if (gravityObjectRB)
        {

            if (collision.GetComponent<PlayerController>() && collision.GetComponent<PlayerController>() != Owner)
            {
                OnHitPlayer(collision.GetComponent<PlayerController>());
                ReturnToPool();
                return;
            }

            if (collision.GetComponent<PlayerController>() && collision.GetComponent<PlayerController>() == Owner)
            {
                return;
            }

            OnHitGORB(gravityObjectRB);
            ReturnToPool();
            return;
        }
    }
}
