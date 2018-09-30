using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamepadPlayerController : PlayerController {
    InputDevice inputDevice;

    void Update()
    {
        if(ControlledPlayer.ForceSelectInputDeviceByIndex)
        {
            inputDevice = MappedInput.InputDevices[ControlledPlayer.InputDeviceIndex];
        }
        else
        {
            //inputDevice = ControlledPlayer.PairedInputDevice;
        }
        

        float leftStickX = inputDevice.GetAxis(MappedAxis.Horizontal);
        Move(leftStickX);

        float rightSitckX = inputDevice.GetAxisRaw(MappedAxis.AimX);
        float rightSitckY = inputDevice.GetAxisRaw(MappedAxis.AimY);
        
        Vector2 aimDir = new Vector2(rightSitckX, rightSitckY);
        AimReticle(aimDir);
        
        if(inputDevice.GetIsAxisTapped(MappedAxis.ChangeGrav) && inputDevice.GetAxis(MappedAxis.ChangeGrav) > 0)
        {
            FlipGravity();
        }
        
        if (inputDevice.GetIsAxisTapped(MappedAxis.ShootGravGun) && aimDir.magnitude > 0)
        {
            ShootGravityGun(aimDir);
        }

        if(inputDevice.GetButtonDown(MappedButton.Dash))
        {
            Dash(aimDir);
        }
    }
}
