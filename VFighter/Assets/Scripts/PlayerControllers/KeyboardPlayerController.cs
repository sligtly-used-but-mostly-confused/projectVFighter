using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardPlayerController : PlayerController
{
    InputDevice inputDevice;

    void Update()
    {        
        if (ControlledPlayer.ForceSelectInputDeviceByIndex)
        {
            inputDevice = MappedInput.InputDevices[ControlledPlayer.InputDeviceIndex];
        }
        else
        {
            inputDevice = ControlledPlayer.PairedInputDevice;
        }

        float mouseX = inputDevice.GetAxisRaw(MappedAxis.AimX);
        float mouseY = inputDevice.GetAxisRaw(MappedAxis.AimY);

        //Debug.Log(inputDevice.GetAxis2DCircleClamp(MappedAxis.AimX, MappedAxis.AimY));

        float Horz = inputDevice.GetAxis(MappedAxis.Horizontal);
        float Vert = inputDevice.GetAxis(MappedAxis.Vertical);
        Move(Horz);
        //Vector2 changeGravDir = new Vector2(ChangeGravX, ChangeGravY);
        //Debug.Log(changeGravDir);
        var mousePos = Camera.main.ScreenToWorldPoint(new Vector2(mouseX, mouseY));
        var aimVector = Vector2.zero;
            
        if(AttachedObject == null)
        { 
            aimVector = mousePos - transform.position;
        }
        else
        {
            aimVector = mousePos - AttachedObject.transform.position;
        }
        
        AimReticle(aimVector);

        if (inputDevice.GetButtonDown(MappedButton.ChangeGrav))
        {
            FlipGravity();
        }

        if (inputDevice.GetButtonDown(MappedButton.ShootGravGun))
        {
            ShootGravityGun(aimVector);
        }
    }
}
