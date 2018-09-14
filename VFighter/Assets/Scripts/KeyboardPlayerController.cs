using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardPlayerController : PlayerController
{
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
}
