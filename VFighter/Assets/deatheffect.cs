using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deatheffect : MonoBehaviour {

    public bool isDead = false;
    [SerializeField]
    public ParticleSystem pulse;
    // Use this for initialization;

    void Start()
    {
        pulse = GetComponent<ParticleSystem>();
    }
    // Update is called once per frame
    void Update()
    {
        if (isDead && !pulse.isPlaying)
        {
            pulse.Play();
            isDead = false;
        }
      
    }
  

}
