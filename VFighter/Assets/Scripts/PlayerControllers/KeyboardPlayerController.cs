using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardPlayerController : PlayerController
{
    void Update()
    {        
        //inputDevice = ControllerSelectManager.Instance.GetPairedInputDevice(PlayerId);

        if(InputDevice == null)
        {
            return;
        }

        if(InputDevice is KeyboardMouseInputDevice)
        {
            Keyboard();
        }
        else
        {
            Gamepad();
        }
    }

    private void Keyboard()
    {
        float mouseX = InputDevice.GetAxisRaw(MappedAxis.AimX);
        float mouseY = InputDevice.GetAxisRaw(MappedAxis.AimY);

        //Debug.Log(inputDevice.GetAxis2DCircleClamp(MappedAxis.AimX, MappedAxis.AimY));

        float Horz = InputDevice.GetAxis(MappedAxis.Horizontal);
        float Vert = InputDevice.GetAxis(MappedAxis.Vertical);
        Move(Horz);
        //Vector2 changeGravDir = new Vector2(ChangeGravX, ChangeGravY);
        //Debug.Log(changeGravDir);
        var mousePos = Camera.main.ScreenToWorldPoint(new Vector2(mouseX, mouseY));
        var aimVector = Vector2.zero;

        if (AttachedObject == null)
        {
            aimVector = mousePos - transform.position;
        }
        else
        {
            aimVector = mousePos - AttachedObject.transform.position;
        }

        AimReticle(aimVector);

        if (InputDevice.GetButtonDown(MappedButton.ChangeGrav))
        {
            FlipGravity();
        }

        if (InputDevice.GetButtonDown(MappedButton.ShootGravGun))
        {
            ShootGravityGun(aimVector);
        }
    }

    private void Gamepad()
    {
        float leftStickX = InputDevice.GetAxis(MappedAxis.Horizontal);
        Move(leftStickX);

        float rightSitckX = InputDevice.GetAxisRaw(MappedAxis.AimX);
        float rightSitckY = InputDevice.GetAxisRaw(MappedAxis.AimY);

        Vector2 aimDir = new Vector2(rightSitckX, rightSitckY);
        AimReticle(aimDir);

        if (InputDevice.GetIsAxisTapped(MappedAxis.ChangeGrav) && InputDevice.GetAxis(MappedAxis.ChangeGrav) > 0)
        {
            FlipGravity();
        }

        if (InputDevice.GetIsAxisTapped(MappedAxis.ShootGravGun) && aimDir.magnitude > 0)
        {
            ShootGravityGun(aimDir);
        }

        if (InputDevice.GetButtonDown(MappedButton.Dash))
        {
            Dash(aimDir);
        }
    }
}
