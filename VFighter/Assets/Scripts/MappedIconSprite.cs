using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MappedIconSprite : MonoBehaviour {

    public MappedButton Button;
    public MappedAxis Axis;
    public InputDevice controller;
    private void Awake()
    {
        //controller = MappedInput.InputDevices[2];
    }

    // Update is called once per frame
    void Update () {
        SelectIcon();
	}

    void SelectIcon() {
        if(!controller)
        {
            return;
        }

        Sprite sprite;
        if (Button == MappedButton.None)
        {
            sprite = controller.GetAxisIcon(Axis);
        }
        else
        {
            sprite = controller.GetButtonIcon(Button);
        }

        if(GetComponent<SpriteRenderer>())
        {
            GetComponent<SpriteRenderer>().sprite = sprite;
        }
        else if(GetComponent<Image>())
        {
            GetComponent<Image>().sprite = sprite;
        }
    }
}
