using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MouseInputDevice : InputDevice
{	
	public override string GetButtonName(MappedButton button)
	{
		var mapping = MappedInput.Instance.MouseInputMapping.GetMouseButtonMapping (button);
		if (mapping != null)
		{
			return string.Format ("Mouse Button {0}", mapping.mouseButtonId);
		}
			
		return "";
	}

	public override string GetAxisName(MappedAxis axis)
	{
		var mapping = MappedInput.Instance.MouseInputMapping.GetMouseAxisMapping (axis);
		if (mapping != null)
		{
			return string.Format ("Mouse Axis {0}", mapping.mouseAxisId);
		}
		return "";
	}

    public override Sprite GetButtonIcon(MappedButton button)
    {
        var mapping = MappedInput.Instance.MouseInputMapping.GetMouseButtonMapping(button);
        if (mapping != null)
        {
            var iconMapping = MappedInput.Instance.MouseInputMapping.MouseIconMappings.GetMouseButtonIconMapping(mapping.mouseButtonId);
            if (iconMapping != null)
                return iconMapping.Icon;
        }

        return null;
    }


    public override Sprite GetAxisIcon(MappedAxis axis)
    {
        // This needs to be changed to reflect the mouse
        var mapping = MappedInput.Instance.MouseInputMapping.GetMouseAxisMapping(axis);
        if (mapping != null)
        {
            return MappedInput.Instance.MouseInputMapping.MouseIconMappings.GetMouseAxisIconMapping(mapping.mouseAxisId).Icon;
        }
        return null;
        
    }

    public override bool GetButton(MappedButton button)
	{
		var mapping = MappedInput.Instance.MouseInputMapping.GetMouseButtonMapping (button);
		if (mapping != null)
		{
			return Input.GetMouseButton (mapping.mouseButtonId);
		}
		return false;
	}

	public override bool GetButtonDown(MappedButton button)
	{
		var mapping = MappedInput.Instance.MouseInputMapping.GetMouseButtonMapping (button);
		if (mapping != null)
		{
			return Input.GetMouseButtonDown (mapping.mouseButtonId);
		}

		return false;
	}

	public override bool GetButtonUp(MappedButton button)
	{
		var mapping = MappedInput.Instance.MouseInputMapping.GetMouseButtonMapping (button);
		if (mapping != null)
		{
			return Input.GetMouseButtonUp (mapping.mouseButtonId);
		}
	
		return false;
	}

	protected override float GetAxisValueRaw (MappedAxis axis)
	{
		float rawVal = 0;
		var mapping = MappedInput.Instance.MouseInputMapping.GetMouseAxisMapping (axis);

		if (mapping != null)
		{
			if (mapping.mouseAxisId == 0)
				rawVal = Input.mousePosition.x;
			else if (mapping.mouseAxisId == 1)
				rawVal = Input.mousePosition.y;
            else if (mapping.mouseAxisId == 2)
                rawVal = Input.mousePosition.y;
            else
				throw new UnityException ("Mouse axis " + mapping.mouseAxisId + " not found");
		}

		return rawVal;
	}

	public override float GetSmoothValue (float lastVal, float currentValRaw, float sensitivity)
	{
		//return base.GetSmoothValue (lastVal, currentValRaw, sensitivity);
		return currentValRaw;
	}
}