using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyTutorialEvent : TutorialEvent
{
    public MappedButton Button;
    public MappedAxis Axis;

    public void Update()
    {
        if(!AttachedPlayer)
        {
            return;
        }

        if(Button != MappedButton.None && AttachedPlayer.InputDevice.GetButtonDown(Button))
        {
            OnFinish.Invoke();
        }

        if(Axis != MappedAxis.None && AttachedPlayer.InputDevice.GetIsAxisTapped(Axis))
        {
            OnFinish.Invoke();
        }
    }
}
