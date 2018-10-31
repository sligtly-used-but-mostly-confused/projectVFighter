using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPromptController : MonoBehaviour {

    public InputDevice Controller;
    public List<TutorialPrompt> Prompts;

    [System.Serializable]
    public struct TutorialPrompt
    {
        [SerializeField]
        public MappedButton MappedButton;
        [SerializeField]
        public MappedAxis MappedAxis;
    }

    private void Start()
    {
        Controller = MappedInput.InputDevices[3];
        GetComponent<Image>().sprite = GetSpriteFromPrompt(Prompts[0]);
    }

    void Update () {
        
        if(Prompts[0].MappedButton != MappedButton.None)
        {
            if (Controller.GetButton(Prompts[0].MappedButton))
            {
                GetComponent<Image>().sprite = GetSpriteFromPrompt(Prompts[1]);
                Prompts.RemoveAt(0);
            }
        }

        if (Prompts[0].MappedAxis != MappedAxis.None)
        {
            if (Controller.GetIsAxisTapped(Prompts[0].MappedAxis))
            {
                GetComponent<Image>().sprite = GetSpriteFromPrompt(Prompts[1]);
                Prompts.RemoveAt(0);
            }
        }
            
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
