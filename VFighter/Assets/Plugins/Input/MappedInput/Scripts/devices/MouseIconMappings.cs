using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class MouseIconMappings : ScriptableObject
{
    public List<ButtonMapping> buttonMappings;
    public List<AxisMapping> axisMappings;

    public ButtonMapping GetButtonMapping(int target)
    {
        for (int i = 0; i < buttonMappings.Count; i++)
        {
            if (buttonMappings[i].mouseButtonIds.Any(x => x == target))
                return buttonMappings[i];
        }

        return null;
    }
    public AxisMapping GetAxisMapping(int target)
    {
        for (int i = 0; i < axisMappings.Count; i++)
        {
            if (axisMappings[i].mouseAxisIds.Any(x => x == target))
                return axisMappings[i];
        }

        return null;
    }

    [System.Serializable]
    public class ButtonMapping
    {
        public int[] mouseButtonIds;
        public Sprite Icon;
    }

    [System.Serializable]
    public class AxisMapping
    {
        public int[] mouseAxisIds;
        public Sprite Icon;
    }
}
