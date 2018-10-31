using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardPlayerController : PlayerController
{
    public void Update()
    {
        if (InputDevice == null)
        {
            return;
        }
        
        Move(this.InputDevice.GetAxis2DCircleClamp(MappedAxis.Horizontal, MappedAxis.Vertical));

        if (InputDevice.GetButtonDown(MappedButton.Ready))
        {
            ToggleReady();
        }
        
        if (InputDevice is KeyboardMouseInputDevice)
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

        if (InputDevice.GetButtonDown(MappedButton.ChangeGrav) && IsReady)
        {
            FlipGravity();
            gc.gravityChange = !gc.gravityChange;
            
        }

        if (InputDevice.GetButtonDown(MappedButton.ShootGravGun))
        {
            ShootGravityGun(aimVector, ProjectileControllerType.Normal);
        }

        if (InputDevice.GetButtonDown(MappedButton.OpenMenu))
        {
            InGameMenuUIManager.Instance.ToggleMenu();
        }

        if (InputDevice.GetButtonDown(MappedButton.Special))
        {
            DoSpecial(aimVector);
        }
    }

    private Vector2 _lastAimDir;

    private void Gamepad()
    {
        float rightSitckX = InputDevice.GetAxisRaw(MappedAxis.AimX);
        float rightSitckY = InputDevice.GetAxisRaw(MappedAxis.AimY);

        Vector2 aimDir = new Vector2(rightSitckX, rightSitckY);
        if(aimDir == Vector2.zero)
        {
            aimDir = _lastAimDir;
        }
        else
        {
            _lastAimDir = aimDir;
        }

        AimReticle(aimDir);

        if (InputDevice.GetIsAxisTapped(MappedAxis.ChangeGrav) && InputDevice.GetAxis(MappedAxis.ChangeGrav) > 0 && IsReady)
        {
            FlipGravity();
            gc.gravityChange = !gc.gravityChange;

        }

        if (InputDevice.GetIsAxisTapped(MappedAxis.ShootGravGun) && aimDir.magnitude > 0)
        {
            ShootGravityGun(aimDir, ProjectileControllerType.Normal);
        }

        if (InputDevice.GetButtonDown(MappedButton.Special))
        {
            DoSpecial(aimDir);
        }
    }
}
