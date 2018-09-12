using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardPlayerController : PlayerController
{
    private Vector2[] _compass = { Vector2.left, Vector2.right, Vector2.up, Vector2.down };

    void Update()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        float verticalLeft = Input.GetAxis("VerticalLeft");
        float horizontalLeft = Input.GetAxis("HorizontalLeft");

        Move(new Vector2(horizontalLeft, verticalLeft));

        MappedInput.inputDevices.

        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var deltaFromPlayer = mousePos - transform.position;

        AimReticle(deltaFromPlayer);

        if(Input.GetKeyDown(KeyCode.Space))
        {
            var closestDir = ClosestDirection(deltaFromPlayer.normalized);
            ChangeGravity(closestDir);
        }
        if (Input.GetButtonDown("Fire1"))
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
