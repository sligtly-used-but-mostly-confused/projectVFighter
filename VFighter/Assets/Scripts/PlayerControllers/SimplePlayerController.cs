using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SimplePlayerController : NetworkBehaviour {
	
	// Update is called once per frame
	void Update () {
        var inputDevice = MappedInput.InputDevices[2];

        if (inputDevice == null)
        {
            return;
        }

        float mouseX = inputDevice.GetAxisRaw(MappedAxis.AimX);
        float mouseY = inputDevice.GetAxisRaw(MappedAxis.AimY);

        //Debug.Log(inputDevice.GetAxis2DCircleClamp(MappedAxis.AimX, MappedAxis.AimY));

        float Horz = inputDevice.GetAxis(MappedAxis.Horizontal);
        float Vert = inputDevice.GetAxis(MappedAxis.Vertical);

        GetComponent<Rigidbody2D>().velocity = new Vector2(Horz, Vert);
    }
}
