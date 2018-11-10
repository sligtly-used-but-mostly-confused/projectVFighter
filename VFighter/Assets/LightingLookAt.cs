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
    protected ParticleSystem ps;
    private GameObject LObject;
    // Use this for initialization
  


    void Start () {
        LObject = Instantiate(prefabLight);
        LObject.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        DontDestroyOnLoad(LObject);
        ps = LObject.GetComponent<ParticleSystem>();
        kp = GetComponent<KeyboardPlayerController>();
    }


    // Update is called once per frame
    void Update () {
      
        var main = ps.main;
        LObject.transform.position = this.transform.position;
        Vector3 positions;
        // Rotate the camera every frame so it keeps looking at the target
        if (kp.AttachedObject)
        {
            positions = kp.AttachedObject.transform.position;
            float dist = Vector3.Distance(LObject.transform.position, positions);
            main.startLifetime = hSliderValue * dist;
            // initialize an array the size of our current particle count
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.particleCount];
            // *pass* this array to GetParticles...
            int num = ps.GetParticles(particles);
            for (int i = 0; i < num; i++)
            {
                float distParticle = Vector3.Distance(LObject.transform.position, particles[i].position);
                if (distParticle > dist) // negative x: make it die
                    particles[i].remainingLifetime = 0;
            }
            // re-assign modified array
            ps.SetParticles(particles, num);
            LObject.transform.LookAt(kp.AttachedObject.transform.position);
            ps.Play();
        }
        else
        {
            positions =  Vector3.zero;
            ps.Stop();
        }
       
    }



  

 
}
