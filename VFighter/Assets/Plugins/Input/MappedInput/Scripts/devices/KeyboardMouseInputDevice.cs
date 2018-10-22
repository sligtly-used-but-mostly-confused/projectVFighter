using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardMouseInputDevice : InputDevice
{
    private Dictionary<MappedAxis, float> MouseAxisPrevValues = new Dictionary<MappedAxis, float>();

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

    public override Sprite GetButtonIcon(MappedButton button)
    {
        var mapping = MappedInput.Instance.GamepadInputMapping.GetGamepadButtonMapping(button);
        return MappedInput.Instance.GamepadInputMapping.IconMapping.GetGamepadButtonIconMapping(mapping.buttons[0]).Icon;
    }

    public override Sprite GetAxisIcon(MappedAxis axis)
    {
        var mapping = MappedInput.Instance.GamepadInputMapping.GetGamepadAxisMapping(axis);
        return MappedInput.Instance.GamepadInputMapping.IconMapping.GetGamepadAxisIconMapping(mapping.axes[0]).Icon;
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
        var MouseMapping = MappedInput.Mouse.GetAxis(axis);
        var KeyboardMapping = MappedInput.KeyBoard.GetAxisRaw(axis);

        return MouseMapping != 0 ? MouseMapping : KeyboardMapping;
    }
}
