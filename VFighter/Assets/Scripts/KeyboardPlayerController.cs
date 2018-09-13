using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardPlayerController : PlayerController
{
    private Vector2[] _compass = { Vector2.left, Vector2.right, Vector2.up, Vector2.down };

    void Update()
    { 
        var inputDevice = MappedInput.InputDevices[2];

        float mouseY = inputDevice.GetAxisRaw(MappedAxis.Vertical);
        float mouseX = inputDevice.GetAxisRaw(MappedAxis.Horizontal);

        var mousePos = Camera.main.ScreenToWorldPoint(new Vector2(mouseX, mouseY));
        var deltaFromPlayer = mousePos - transform.position;

        AimReticle(deltaFromPlayer);

        if(inputDevice.GetButtonDown(MappedButton.ChangeGrav))
        {
            var closestDir = ClosestDirection(deltaFromPlayer.normalized);
            ChangeGravity(closestDir);
        }
        if (inputDevice.GetButtonDown(MappedButton.ShootGravGun))
        {
            ShootGravityGun(deltaFromPlayer);
        }
    }

    Vector2 ClosestDirection(Vector2 v)
    {
 
        var maxDot = -Mathf.Infinity;
        var ret = Vector3.zero;
     
        foreach(var dir in _compass)
        { 
            var t = Vector3.Dot(v, dir);
            if (t > maxDot)
            {
                ret = dir;
                maxDot = t;
            }
        }
 
        return ret;
    }
}
