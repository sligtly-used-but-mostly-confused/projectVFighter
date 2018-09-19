using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamepadPlayerController : PlayerController {
    [SerializeField]
    private int _inputDevice = 2;

    InputDevice inputDevice;
    private float _triggerDeadZone;
    List<float> TriggerPastVals = new List<float>();

    private void Start()
    {
        inputDevice = MappedInput.InputDevices[_inputDevice];
    }

    void Update()
    {
        inputDevice.Center = transform.position;

        float rightSitckX = inputDevice.GetAxisRaw(MappedAxis.AimX);
        float rightSitckY = inputDevice.GetAxisRaw(MappedAxis.AimY);

        float leftStickX = inputDevice.GetAxis(MappedAxis.Horizontal);

        Move(leftStickX);
        Vector2 aimDir = new Vector2(rightSitckX, rightSitckY);
        
        AimReticle(aimDir);
        
        if(inputDevice.GetButtonDown(MappedButton.ChangeGrav))
        {
            FlipGravity();
        }

        if (IsTriggerTapped(MappedAxis.ShootGravGun) && aimDir.magnitude > 0)
        {
            ShootGravityGun(aimDir);
        }
    }

    bool IsTriggerTapped(MappedAxis axis)
    {
        float val = inputDevice.GetAxis(axis);
        TriggerPastVals.Add(val);

        if (TriggerPastVals.Count > 3)
        {
            TriggerPastVals.RemoveAt(0);
        }

        if(TriggerPastVals.Count < 3)
        {
            return false;
        }

        return TriggerPastVals[0] >= TriggerPastVals[1] && TriggerPastVals[1] < TriggerPastVals[2];
    }
}
