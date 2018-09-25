﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamepadPlayerController : PlayerController {
    InputDevice inputDevice;
    private float _triggerDeadZone;
    Dictionary<MappedAxis,List<float>> TriggerPastVals = new Dictionary<MappedAxis, List<float>>();

    void Update()
    {
        inputDevice = MappedInput.InputDevices[ControlledPlayer.InputDeviceIndex];

        float leftStickX = inputDevice.GetAxis(MappedAxis.Horizontal);
        Move(leftStickX);

        float rightSitckX = inputDevice.GetAxisRaw(MappedAxis.AimX);
        float rightSitckY = inputDevice.GetAxisRaw(MappedAxis.AimY);

        Vector2 aimDir = new Vector2(rightSitckX, rightSitckY);
        //Debug.Log(aimDir + " pre");
        AimReticle(aimDir);
        
        if(IsAxisTapped(MappedAxis.ChangeGrav) && inputDevice.GetAxis(MappedAxis.ChangeGrav) > 0)
        {
            FlipGravity();
        }
        
        if (IsAxisTapped(MappedAxis.ShootGravGun) && aimDir.magnitude > 0)
        {
            Debug.Log(aimDir);
            ShootGravityGun(aimDir);
        }

        if(inputDevice.GetButtonDown(MappedButton.Dash))
        {
            Dash(aimDir);
        }
    }

    bool IsAxisTapped(MappedAxis axis)
    {
        float val = inputDevice.GetAxis(axis);
        
        if(!TriggerPastVals.ContainsKey(axis))
        {
            TriggerPastVals.Add(axis, new List<float>());
        }

        TriggerPastVals[axis].Add(val);

        if (TriggerPastVals[axis].Count > 3)
        {
            TriggerPastVals[axis].RemoveAt(0);
        }

        if(TriggerPastVals[axis].Count < 3)
        {
            return false;
        }

        return TriggerPastVals[axis][0] >= TriggerPastVals[axis][1] && TriggerPastVals[axis][1] < TriggerPastVals[axis][2];
    }
}
