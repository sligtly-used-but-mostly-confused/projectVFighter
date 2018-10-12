using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateEffect : MonoBehaviour {
    [SerializeField]
    public ForceZoneController other;
    [SerializeField]
    float angleRotation = 360;
    [SerializeField]
    SpriteRenderer im;
    private ParticleSystem ps;
    private Vector2 forces;
    // Use this for initialization
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        other = GetComponent<ForceZoneController>();
        im = GetComponent<SpriteRenderer>();
        im.enabled = false;
        
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
