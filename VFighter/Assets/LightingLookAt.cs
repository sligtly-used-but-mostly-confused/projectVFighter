using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingLookAt : MonoBehaviour {
 
    [SerializeField]
    public float hSliderValue = 0.01f;
    [SerializeField]
    public PlayerController _attachedPlayer;
    private ParticleSystem ps;
    
    // Use this for initialization
    void Start () {
        ps = GetComponent<ParticleSystem>();
    }

    public void ConnectToPlayer(PlayerController player)
    {
        _attachedPlayer = player;
    }

    public void DisconnectPlayer()
    {
        _attachedPlayer = null;
    }

    // Update is called once per frame
    void Update () {
        var main = ps.main;
        Vector3 positions;
        // Rotate the camera every frame so it keeps looking at the target
        if (_attachedPlayer)
        {
            positions = _attachedPlayer.transform.position;

        }
        else
        {
            positions =  Vector3.zero;
        }
        float dist = Vector3.Distance(this.transform.position, positions);
        main.startLifetime = hSliderValue * dist;
        // initialize an array the size of our current particle count
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.particleCount];
        // *pass* this array to GetParticles...
        int num = ps.GetParticles(particles);
        for (int i = 0; i < num; i++)
        {
            float distParticle = Vector3.Distance(this.transform.position, particles[i].position);
            if (distParticle > dist) // negative x: make it die
                particles[i].remainingLifetime = 0;
        }
        // re-assign modified array
        ps.SetParticles(particles, num);
        transform.LookAt(_attachedPlayer.transform.position);
    }



  

 
}
