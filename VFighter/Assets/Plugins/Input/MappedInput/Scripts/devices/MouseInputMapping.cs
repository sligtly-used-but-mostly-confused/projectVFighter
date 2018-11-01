using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MouseInputMapping : ScriptableObject
{
	public List<ButtonMapping> mouseButtonMappings = new List<ButtonMapping>();
    public List<AxisMapping> mouseAxisMappings = new List<AxisMapping>();

    public MouseIconMappings MouseIconMappings;

    public ButtonMapping GetMouseButtonMapping(MappedButton target)
	{
		for (int i = 0; i < mouseButtonMappings.Count; i++)
		{
			if( mouseButtonMappings[i].target == target )
				return mouseButtonMappings[i];
		}

		return null;
	}

	public AxisMapping GetMouseAxisMapping(MappedAxis target)
	{
		for (int i = 0; i < mouseAxisMappings.Count; i++)
		{
			if( mouseAxisMappings[i].target == target )
				return mouseAxisMappings[i];
		}

		return null;
	}

    [System.Serializable]
	public class ButtonMapping
	{
		public MappedButton target;
		public int mouseButtonId;
	}

	[System.Serializable]
	public class AxisMapping
	{
		public MappedAxis target;
		public int mouseAxisId;
	}
}	