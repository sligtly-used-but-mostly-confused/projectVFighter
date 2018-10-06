using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if(ControllerSelectManager.Instance)
        {
            ControllerSelectManager.Instance.ClearUsedInputDevices();
        }
	}
}
