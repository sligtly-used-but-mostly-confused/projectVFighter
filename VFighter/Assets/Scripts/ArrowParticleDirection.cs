using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ArrowParticleDirection : MonoBehaviour {
    public ForceZoneController other;
    private ParticleSystem ps;
    private Vector2 forces;
	// Use this for initialization
	void Start () {
        ps = GetComponent<ParticleSystem>();
        other = GetComponent<ForceZoneController>();
        float xValue = other.GetgravityForce().x;
        float yValue = other.GetgravityForce().y;
        ParticleSystem.MainModule mainMod = ps.main;
        mainMod.startRotation = Mathf.Atan2(xValue, yValue);
	}
	

}
