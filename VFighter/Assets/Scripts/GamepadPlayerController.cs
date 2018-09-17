using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamepadPlayerController : PlayerController {
    [SerializeField]
    private int _inputDevice = 2;

    void Update()
    {
        var inputDevice = MappedInput.InputDevices[_inputDevice];

        float mouseX = inputDevice.GetAxisRaw(MappedAxis.AimX);
        float mouseY = inputDevice.GetAxisRaw(MappedAxis.AimY);

        float ChangeGravX = inputDevice.GetAxis(MappedAxis.Horizontal);
        float ChangeGravY = inputDevice.GetAxis(MappedAxis.Vertical);

        Vector2 changeGravDir = new Vector2(ChangeGravX, ChangeGravY);
        Debug.Log(changeGravDir);
        //Debug.Log(changeGravDir);
        var mousePos = Camera.main.ScreenToWorldPoint(new Vector2(mouseX, mouseY));
        var deltaFromPlayer = mousePos - transform.position;

        AimReticle(deltaFromPlayer);

        //if(inputDevice.GetButtonDown(MappedButton.ChangeGrav))
        if (changeGravDir != Vector2.zero)
        {
            var closestDir = ClosestDirection(changeGravDir.normalized);
            ChangeGravity(closestDir);
        }
        if (inputDevice.GetButtonDown(MappedButton.ShootGravGun))
        {
            ShootGravityGun(deltaFromPlayer);
        }
    }
}
