using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KeyboardInputDevice : InputDevice
{	
	public override string GetButtonName(MappedButton button)
	{
        var mapping = MappedInput.Instance.KeyboardInputMapping.GetKeyboardButtonMapping(button);
        if (mapping == null)
			return "";

		return mapping.buttons[0].ToString();
	}

	public override string GetAxisName(MappedAxis axis)
	{
        var mapping = MappedInput.Instance.KeyboardInputMapping.GetKeyboardAxisMapping(axis);
        if (mapping != null) {
		
			if (mapping.buttonsPositive.Length > 0 && mapping.buttonsNegative.Length > 0)
				return string.Format ("{0}/{1}", mapping.buttonsPositive [0].ToString (), mapping.buttonsNegative [0].ToString ());
			if (mapping.buttonsPositive.Length > 0)
				return string.Format ("{0}", mapping.buttonsPositive [0].ToString ());
		}
		return "no binding";
	}

    public override Sprite GetButtonIcon(MappedButton button)
    {
        var mapping = MappedInput.Instance.KeyboardInputMapping.GetKeyboardButtonMapping(button);
        return MappedInput.Instance.KeyboardInputMapping.IconMappings.GetKeyboardButtonIconMapping(mapping.buttons[0]).Icon;
    }

    public override Sprite GetAxisIcon(MappedAxis axis)
    {
        //This needs to be able to return both the positive and negative button icon
        var mapping = MappedInput.Instance.KeyboardInputMapping.GetKeyboardAxisMapping(axis);
       return MappedInput.Instance.KeyboardInputMapping.IconMappings.GetKeyboardAxisIconMapping(mapping.axes[0]).Icon;
        //if (mapping != null)
        //{
        //    if (mapping.buttonsPositive.Length > 0 && mapping.buttonsNegative.Length > 0)
        //        return MappedInput.Instance.KeyboardInputMapping.IconMappings.GetKeyboardAxisIconMapping(mapping.buttonsNegative[0]).Icon;
        //    if (mapping.buttonsPositive.Length > 0)
        //        return MappedInput.Instance.KeyboardInputMapping.IconMappings.GetKeyboardAxisIconMapping(mapping.buttonsPositive[0]).Icon;
        //}
        //return null;
    }

    public override bool GetButton(MappedButton button)
	{
        var mapping = MappedInput.Instance.KeyboardInputMapping.GetKeyboardButtonMapping(button);
		if (mapping == null)
			return false;
		
		for(int i = 0; i < mapping.buttons.Length; i++)
		{
			if( Input.GetKey(mapping.buttons[i]) )
				return true;
		}
		return false;
	}


	public override bool GetButtonDown(MappedButton button)
	{
        var mapping = MappedInput.Instance.KeyboardInputMapping.GetKeyboardButtonMapping(button);
		if (mapping == null)
			return false;
		
		for(int i = 0; i < mapping.buttons.Length; i++)
		{
			if( Input.GetKeyDown(mapping.buttons[i]) )
				return true;
		}

		return false;
	}

	public override bool GetButtonUp(MappedButton button)
	{
        var mapping = MappedInput.Instance.KeyboardInputMapping.GetKeyboardButtonMapping (button);
		if (mapping == null)
			return false;
		
		for(int i = 0; i < mapping.buttons.Length; i++)
		{
			if( Input.GetKeyUp(mapping.buttons[i]) )
				return true;
		}
		return false;
	}

	protected override float GetAxisValueRaw (MappedAxis axis)
	{

        var mapping = MappedInput.Instance.KeyboardInputMapping.GetKeyboardAxisMapping (axis);

		if (mapping == null)
			return 0;

		float rawVal = 0;
		for(int i = 0; i < mapping.buttonsPositive.Length; i++)
		{
			if( Input.GetKey(mapping.buttonsPositive[i]) )
				rawVal++;
		}

		for(int i = 0; i < mapping.buttonsNegative.Length; i++)
		{
            if (Input.GetKey(mapping.buttonsNegative[i]))
                rawVal--;
		}

        return Mathf.Clamp (rawVal, -1, 1);
	}

}