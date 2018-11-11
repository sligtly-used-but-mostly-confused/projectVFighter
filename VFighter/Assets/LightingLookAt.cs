using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingLookAt : MonoBehaviour {

    [SerializeField]
    public float hSliderValue = 0.01f;
    [SerializeField]
    public KeyboardPlayerController kp;
    [SerializeField]
    protected GameObject prefabLight;
    [SerializeField]
    protected ParticleSystem[] ps;
    private GameObject LObject;
    // Use this for initialization
  


    void Start () {
        LObject = Instantiate(prefabLight);
       
        DontDestroyOnLoad(LObject);
        ps = LObject.GetComponentsInChildren<ParticleSystem>();
        kp = GetComponent<KeyboardPlayerController>();
        LObject.transform.position = this.transform.position;
    }


    // Update is called once per frame
    void Update () {
      
        var main1 = ps[0].main;
        var main2 = ps[1].main;
        var main3 = ps[1].main;
        LObject.transform.position = this.transform.position;
        Vector3 positions;
        // Rotate the camera every frame so it keeps looking at the target
        if (kp.AttachedObject)
        {
            positions = kp.AttachedObject.transform.position;
            float dist = Vector3.Distance(LObject.transform.position, positions);
            main1.startLifetime = hSliderValue * dist;
            main2.startLifetime = hSliderValue * dist;
            main3.startLifetime = hSliderValue * dist;
            // initialize an array the size of our current particle count
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps[0].particleCount];
            ParticleSystem.Particle[] particles1 = new ParticleSystem.Particle[ps[1].particleCount];
            ParticleSystem.Particle[] particles2 = new ParticleSystem.Particle[ps[2].particleCount];
            // *pass* this array to GetParticles...
            int num = ps[0].GetParticles(particles);
            for (int i = 0; i < num; i++)
            {
                float distParticle = Vector3.Distance(LObject.transform.position, particles[i].position);
                float distParticle1 = Vector3.Distance(LObject.transform.position, particles[i].position);
                if (distParticle > dist) 
                    particles[i].remainingLifetime = 0;
                if (distParticle1 > dist) 
                    particles2[i].remainingLifetime = 0;
            }
            int num1 = ps[1].GetParticles(particles1);
            for (int i = 0; i < num1; i++)
            {
                float distParticle = Vector3.Distance(LObject.transform.position, particles1[i].position);
                if (distParticle > dist) // negative x: make it die
                    particles1[i].remainingLifetime = 0;
            }
            // re-assign modified array
            ps[0].SetParticles(particles, num);
            ps[1].SetParticles(particles1, num1);
            ps[2].SetParticles(particles2, num);
            LObject.transform.LookAt(kp.AttachedObject.transform.position);
            ps[0].Play();
            ps[1].Play();
            ps[2].Play();
        }
        else
        {
            positions =  Vector3.zero;
            ps[0].Stop();
            ps[1].Stop();
            ps[2].Stop();
        }
       
    }



  

 
}
