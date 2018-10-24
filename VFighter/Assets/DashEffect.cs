using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashEffect : MonoBehaviour {
   
    public bool dashOn = false;
    [SerializeField]
    public ParticleSystem pulse;
    // Use this for initialization;
    bool prevState = true;

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
    public bool dashEffectOn(){
        return pulse.isPlaying;
    }
}
