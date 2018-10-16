using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateEffect : MonoBehaviour {
    [SerializeField]
    float angleRotation = 360;
    [SerializeField]
    private ParticleSystem ps;
    private Vector2 forces;
    // Use this for initialization
    void Start()
    {
    }


    // Update is called once per frame
    void FixedUpdate () {
		ParticleSystem.MainModule mainMod = ps.main;
        if(angleRotation < 0)
        {
            angleRotation = 360.0f;
        }
        angleRotation += Time.deltaTime * 1;
        mainMod.startRotation = angleRotation;
	}
}
