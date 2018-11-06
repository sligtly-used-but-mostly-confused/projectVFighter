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

        if (InputDevice.GetButtonDown(MappedButton.OpenMenu))
        {
            InGameMenuUIManager.Instance.ToggleMenu();
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

        if (InputDevice.GetButtonDown(MappedButton.ChangeGrav))
        {
            FlipGravity();
        }

        if (InputDevice.GetIsAxisTappedPos(MappedAxis.ChangeGravAxis))
        {
            ChangeGravityTowardsDir(Vector2.up);
        }

        if (InputDevice.GetIsAxisTappedNeg(MappedAxis.ChangeGravAxis))
        {
            ChangeGravityTowardsDir(Vector2.down);
        }

        if (InputDevice.GetButtonDown(MappedButton.ShootGravGun))
        {
            ShootGravityGun(aimVector, ProjectileControllerType.Normal);
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

        if (InputDevice.GetIsAxisTappedPos(MappedAxis.ChangeGravAxis, .5f))
        {
            ChangeGravityTowardsDir(Vector2.up);
        }

        if (InputDevice.GetIsAxisTappedNeg(MappedAxis.ChangeGravAxis, -.5f))
        {
            ChangeGravityTowardsDir(Vector2.down);
        }

        if (InputDevice.GetIsAxisTappedPos(MappedAxis.ChangeGrav) && InputDevice.GetAxis(MappedAxis.ChangeGrav) > 0)
        {
            FlipGravity();
        }

        if (InputDevice.GetIsAxisTappedPos(MappedAxis.ShootGravGun) && aimDir.magnitude > 0)
        {
            ShootGravityGun(aimDir, ProjectileControllerType.Normal);
        }

        if (InputDevice.GetButtonDown(MappedButton.Special))
        {
            DoSpecial(aimDir);
        }
    }
}
