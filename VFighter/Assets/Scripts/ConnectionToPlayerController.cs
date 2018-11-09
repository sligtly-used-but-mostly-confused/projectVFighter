using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionToPlayerController : MonoBehaviour {
    [SerializeField]
    ParticleSystem ps;

    private float hSliderValue = 0.014f;

    public PlayerController _attachedPlayer;

    void Start()
    {
        ps = GetComponentInChildren<ParticleSystem>();
        _attachedPlayer = GetComponentInParent<PlayerController>();
    }

    public void ConnectToPlayer(PlayerController player)
    {
        _attachedPlayer = player;
    }

    public void DisconnectPlayer()
    {
        _attachedPlayer = null;
    }

    void Update()
    {
        var main = ps.main;
        Vector3 positions;
        // Rotate the camera every frame so it keeps looking at the target
        if (_attachedPlayer)
        {
            positions = _attachedPlayer.transform.position;
            ps.Play();

        }
        else
        {
            positions = Vector3.zero;
            ps.Stop();
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
