using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MappedIconSprite : MonoBehaviour {

    public MappedButton Button;
    public MappedAxis Axis;
    public InputDevice controller;
    private void Awake()
    {
        controller = MappedInput.InputDevices[2];
    }

    // Update is called once per frame
    void Update () {
        SelectIcon();
        //GetComponent<SpriteRenderer>().sprite = controller.GetButtonIcon(Button);
        //GetComponent<SpriteRenderer>().sprite = controller.GetAxisIcon(Axis);
	}

    void SelectIcon() {
        if (Button == MappedButton.None)
        {
            GetComponent<SpriteRenderer>().sprite = controller.GetAxisIcon(Axis);
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = controller.GetButtonIcon(Button);
        }
    }
}
