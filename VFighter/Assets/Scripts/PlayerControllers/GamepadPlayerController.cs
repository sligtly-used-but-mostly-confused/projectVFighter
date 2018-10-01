using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamepadPlayerController : PlayerController {

    void Update()
    {
        //inputDevice = ControllerSelectManager.Instance.GetPairedInputDevice(ControlledPlayer.NetworkControllerId);
        if (InputDevice == null)
        {
            return;
        }

        float leftStickX = InputDevice.GetAxis(MappedAxis.Horizontal);
        Move(leftStickX);

        float rightSitckX = InputDevice.GetAxisRaw(MappedAxis.AimX);
        float rightSitckY = InputDevice.GetAxisRaw(MappedAxis.AimY);
        
        Vector2 aimDir = new Vector2(rightSitckX, rightSitckY);
        AimReticle(aimDir);
        
        if(InputDevice.GetIsAxisTapped(MappedAxis.ChangeGrav) && InputDevice.GetAxis(MappedAxis.ChangeGrav) > 0)
        {
            FlipGravity();
        }
        
        if (InputDevice.GetIsAxisTapped(MappedAxis.ShootGravGun) && aimDir.magnitude > 0)
        {
            ShootGravityGun(aimDir);
        }

        if(InputDevice.GetButtonDown(MappedButton.Dash))
        {
            Dash(aimDir);
        }
    }


}
