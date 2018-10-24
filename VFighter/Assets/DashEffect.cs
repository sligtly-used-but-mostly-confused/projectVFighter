using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashEffect : MonoBehaviour {
    [SerializeField]
    bool dashOn = false;
    [SerializeField]
    private ParticleSystem pulse;
    // Use this for initialization;
    public bool prevState = true;

    void Start()
    {
        pulse = GetComponent<ParticleSystem>();
    }
	// Update is called once per frame
    void Update()
    {
        if(dashOn != prevState)
        {
            if (dashOn)
            {
                pulse.Play();
            }
            else
            {
                pulse.Stop();
            }
        }
        prevState = dashOn;
        
    }
}
