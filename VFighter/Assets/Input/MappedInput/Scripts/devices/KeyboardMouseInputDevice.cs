using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardMouseInputDevice : InputDevice
{
    public override string GetAxisName(MappedAxis axis)
    {
        var MouseMapping = MappedInput.Mouse.GetAxisName(axis);
        var KeyboardMapping = MappedInput.KeyBoard.GetAxisName(axis);

        var mapping = MouseMapping != "" ? MouseMapping : KeyboardMapping;

        return mapping;
    }

    public override bool GetButton(MappedButton button)
    {
        var MouseMapping = MappedInput.Mouse.GetButton(button);
        var KeyboardMapping = MappedInput.KeyBoard.GetButton(button);

        return MouseMapping || KeyboardMapping;
    }

    public override bool GetButtonDown(MappedButton button)
    {
        var MouseMapping = MappedInput.Mouse.GetButtonDown(button);
        var KeyboardMapping = MappedInput.KeyBoard.GetButtonDown(button);

        return MouseMapping || KeyboardMapping;
    }

    public override string GetButtonName(MappedButton button)
    {
        var MouseMapping = MappedInput.Mouse.GetButtonName(button);
        var KeyboardMapping = MappedInput.KeyBoard.GetButtonName(button);

        var mapping = MouseMapping != "" ? MouseMapping : KeyboardMapping;

        return mapping;
    }

    public override bool GetButtonUp(MappedButton button)
    {
        var MouseMapping = MappedInput.Mouse.GetButtonUp(button);
        var KeyboardMapping = MappedInput.KeyBoard.GetButtonUp(button);

        return MouseMapping || KeyboardMapping;
    }

    protected override float GetAxisValueRaw(MappedAxis axis)
    {
        
        var MouseMapping = MappedInput.Mouse.GetAxisRaw(axis);
        var KeyboardMapping = MappedInput.KeyBoard.GetAxisRaw(axis);
        //Debug.Log(axis + " " + MouseMapping + " " + KeyboardMapping);
        return Mathf.Max(MouseMapping, KeyboardMapping);
    }
}
