using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu]
public class GamepadIconMappings : ScriptableObject
{
    public List<ButtonIconMapping> buttonMappings = new List<ButtonIconMapping>();
    public List<AxisIconMapping> axisMappings = new List<AxisIconMapping>();

    public ButtonIconMapping GetGamepadButtonIconMapping(GamepadButton target)
    {
        for (int i = 0; i < buttonMappings.Count; i++)
        {
            if (buttonMappings[i].Button == target)
                return buttonMappings[i];
        }

        return null;
    }
    public AxisIconMapping GetGamepadAxisIconMapping(GamepadAxis target)
    {
        for (int i = 0; i < axisMappings.Count; i++)
        {
            if (axisMappings[i].Axes.Any(x => x == target))
                return axisMappings[i];
        }

        return null;
    }

    [System.Serializable]
    public class ButtonIconMapping
    {
        public GamepadButton Button;
        public Sprite Icon;
    }

    [System.Serializable]
    public class AxisIconMapping
    {
        public GamepadAxis[] Axes;
        public Sprite IconNegative;
        public Sprite IconNeutral;
        public Sprite IconPositive;
    }
}
