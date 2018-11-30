using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MappedIconSprite : MonoBehaviour {

    public MappedButton Button;
    public InputDevice controller;
    private void Awake()
    {
        controller = MappedInput.InputDevices[2];
    }

    // Update is called once per frame
    void Update () {
        if(GetComponent<SpriteRenderer>())
        {
            GetComponent<SpriteRenderer>().sprite = controller.GetButtonIcon(Button);
        }

        if(GetComponent<Image>())
        {
            GetComponent<Image>().sprite = controller.GetButtonIcon(Button);
        }
	}
}
