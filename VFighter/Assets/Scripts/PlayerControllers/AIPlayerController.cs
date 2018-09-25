using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayerController : PlayerController
{
    protected override void Awake()
    {
        base.Awake();
    }
    void Start()
    {
        //StartCoroutine(DoAIMove());
    }

    IEnumerator DoAIMove()
    {
        //AimReticle
        Vector2 dir = Random.onUnitSphere;
        dir = dir.normalized;

        AimReticle(dir);
        var chance = Random.value;

        if(chance < .1f)
        {
            ChangeGravity(dir);
        }
        else
        {
            //ShootGravityGun
            ShootGravityGun(dir);
        }

        yield return new WaitForSeconds(.1f);

        yield return DoAIMove();
    }
}
