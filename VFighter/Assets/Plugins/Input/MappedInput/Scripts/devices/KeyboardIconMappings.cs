﻿using System.Collections;
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

    // I'm not sure that we need this at all because the keyboard doesn't really
    // have an axis
    
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

    // This kind of needs to parallel the gamepad
    [System.Serializable]
    public class AxisIconMapping
    {
        public KeyboardAxis[] Axes;
        public Sprite Icon;
        //public MappedAxis target;
        //public KeyboardAxis[] axes;
        //public KeyboardButton[] buttonsPositive;
        //public KeyboardButton[] buttonsNegative;
        //public bool inverted;

    }
}
