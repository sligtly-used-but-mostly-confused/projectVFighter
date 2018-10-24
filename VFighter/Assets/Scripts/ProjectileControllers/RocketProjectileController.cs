﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RocketProjectileController : GravityGunProjectileController
{
    public GameObject RocketBlastPrefab;
    public override void OnHitGORB(GravityObjectRigidBody GORB)
    {
        //var blast = Instantiate(RocketBlastPrefab);
        //blast.transform.position = this.transform.position;
        //NetworkServer.Spawn(blast);
    }

    private void OnDestroy()
    {
        var blast = Instantiate(RocketBlastPrefab);
        blast.transform.position = this.transform.position;
        NetworkServer.Spawn(blast);
    }
}
