using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamepadPlayerController : PlayerController {
    [SerializeField]
    private int _inputDevice = 2;

    void Update()
    {
        var inputDevice = MappedInput.InputDevices[_inputDevice];

        float rightSitckX = inputDevice.GetAxisRaw(MappedAxis.AimX);
        float rightSitckY = inputDevice.GetAxisRaw(MappedAxis.AimY);

        float leftStickX = inputDevice.GetAxis(MappedAxis.Horizontal);

        Move(leftStickX);
        Vector2 aimDir = new Vector2(rightSitckX, rightSitckY).normalized;
        
        AimReticle(aimDir);
        
        if(inputDevice.GetButtonDown(MappedButton.ChangeGrav))
        {
            FlipGravity();
        }

        if (inputDevice.GetButtonDown(MappedButton.ShootGravGun))
        {
            ShootGravityGun(aimDir);
        }
    }
}
