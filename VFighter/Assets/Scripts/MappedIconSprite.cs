using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MappedIconSprite : MonoBehaviour {

    public MappedButton Button;
    public InputDevice controller;
    private void Awake()
    {
        controller = MappedInput.InputDevices[2];
    }

    // Update is called once per frame
    void Update () {
        GetComponent<SpriteRenderer>().sprite = controller.GetButtonIcon(Button);
	}
}
