using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAndUseSpecialTutorialEvent : TutorialEvent
{

    public PlayerCharacterType TargetType;

	
	void Update ()
    {
        if (AttachedPlayer && AttachedPlayer.InputDevice.GetButtonDown(MappedButton.Special) && AttachedPlayer.CharacterType == TargetType)
        {
            OnFinish.Invoke();
        }
    }
}
