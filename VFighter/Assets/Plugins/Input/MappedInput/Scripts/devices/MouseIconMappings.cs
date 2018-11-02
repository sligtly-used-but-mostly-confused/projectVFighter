using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class MouseIconMappings : ScriptableObject
{
    public List<ButtonIconMapping> buttonMappings = new List<ButtonIconMapping>();
    public List<AxisIconMapping> axisMappings = new List<AxisIconMapping>();

    public ButtonIconMapping GetMouseButtonIconMapping(int target)
    {
        for (int i = 0; i < buttonMappings.Count; i++)
        {
            if (buttonMappings[i].mouseButtonIds.Any(x => x == target))
                return buttonMappings[i];
        }

        return null;
    }
    public AxisIconMapping GetMouseAxisIconMapping(int target)
    {
        for (int i = 0; i < axisMappings.Count; i++)
        {
            if (axisMappings[i].mouseAxisIds.Any(x => x == target))
                return axisMappings[i];
        }

        return null;
    }

    [System.Serializable]
    public class ButtonIconMapping
    {
        public int[] mouseButtonIds;
        public Sprite Icon;
    }

    [System.Serializable]
    public class AxisIconMapping
    {
        public int[] mouseAxisIds;
        public Sprite Icon;
    }
}
