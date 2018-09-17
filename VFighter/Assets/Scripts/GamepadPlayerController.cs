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
        //Debug.Log(leftStickX);
        Move(new Vector2(leftStickX, 0));
        //float ChangeGravY = inputDevice.GetAxis(MappedAxis.Vertical);

        //Vector2 changeGravDir = new Vector2(ChangeGravX, ChangeGravY);
        
        Vector2 aimDir = new Vector2(rightSitckX, rightSitckY).normalized;
        
        AimReticle(aimDir);

        //if(inputDevice.GetButtonDown(MappedButton.ChangeGrav))
        //if (changeGravDir != Vector2.zero)
        //{
        //    var closestDir = ClosestDirection(changeGravDir.normalized);
        //    ChangeGravity(closestDir);
        //}
        
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
