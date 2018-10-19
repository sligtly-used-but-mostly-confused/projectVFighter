using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu]
public class KeyboardIconMappings : ScriptableObject
{

    public List<ButtonIconMapping> buttonMappings = new List<ButtonIconMapping>();

    public ButtonIconMapping GetGamepadButtonIconMapping(KeyCode target)
    {
        for (int i = 0; i < buttonMappings.Count; i++)
        {
            if (buttonMappings[i].Keys.Any( x => (x == target)))
                return buttonMappings[i];
        }

        return null;
    }

    [System.Serializable]
    public class ButtonIconMapping
    {
        public KeyCode[] Keys;
        public Sprite Icon;
    }
}
