using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu]
public class KeyboardIconMappings : ScriptableObject
{
    public List<ButtonIconMapping> buttonMappings = new List<ButtonIconMapping>();
    public List<AxisIconMapping> axisMappings = new List<AxisIconMapping>();

    public ButtonIconMapping GetKeyboardButtonIconMapping(KeyCode target)
    {
        for (int i = 0; i < buttonMappings.Count; i++)
        {
            if (buttonMappings[i].Keys.Any( x => (x == target)))
                return buttonMappings[i];
        }

        return null;
    }
    
    public AxisIconMapping GetKeyboardAxisIconMapping(KeyboardAxis target)
    {
        for (int i = 0; i < axisMappings.Count; i++)
        {
            if (axisMappings[i].Axes.Any(x => x == target))
            {
                return axisMappings[i];
            }
        }
        return null;
    }

    [System.Serializable]
    public class ButtonIconMapping
    {
        public KeyCode[] Keys;
        public Sprite Icon;
    }
    
    [System.Serializable]
    public class AxisIconMapping
    {
        public KeyboardAxis[] Axes;
        public Sprite Icon;
    }
}
