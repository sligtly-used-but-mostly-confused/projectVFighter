using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TutorialPromptController : MonoBehaviour
{
    public InputDevice Controller;
    public List<TutorialPrompt> Prompts;
    public PlayerController AttachedPlayer;

    [System.Serializable]
    public struct TutorialPrompt
    {
        [SerializeField]
        public MappedButton MappedButton;
        [SerializeField]
        public MappedAxis MappedAxis;
    }

    void Update () {

        Controller = AttachedPlayer.InputDevice;
        if(Controller && GetComponent<SpriteRenderer>().sprite == null)
        {
            GetComponent<SpriteRenderer>().sprite = GetSpriteFromPrompt(Prompts[0]);
        }
        
        if (Controller && Prompts[0].MappedButton != MappedButton.None)
        {
            if (Controller.GetButton(Prompts[0].MappedButton))
            {
                OnPromtNext();
            }
        }
        
        if (Controller && Prompts[0].MappedAxis != MappedAxis.None)
        {
            if (Controller.GetIsAxisTapped(Prompts[0].MappedAxis))
            {
                OnPromtNext();
            }
        }
    }

    private void OnPromtNext()
    {
        if (Prompts.Count == 1)
        {
            gameObject.SetActive(false);
            return;
        }

        GetComponent<SpriteRenderer>().sprite = GetSpriteFromPrompt(Prompts[1]);
        Prompts.RemoveAt(0);
    }

    private Sprite GetSpriteFromPrompt(TutorialPrompt prompt)
    {
        Sprite icon = null;
        
        if(prompt.MappedButton != MappedButton.None)
        {
            icon = Controller.GetButtonIcon(prompt.MappedButton);
        }

        if (prompt.MappedAxis != MappedAxis.None && icon == null)
        {
            icon = Controller.GetAxisIcon(prompt.MappedAxis);
        }

        return icon;
    }
}
