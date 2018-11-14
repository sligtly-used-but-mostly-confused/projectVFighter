using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gg_GrabAudio : MonoBehaviour {
    [SerializeField]
    public ParticleSystem ps;
    [SerializeField]
    public AudioSource ad;
    // Use this for initialization
    void Start () {
        ps = GetComponent<ParticleSystem>();
        ad = GetComponent<AudioSource>();
	}

    // Update is called once per frame
    void Update()
    {
        var main = ps.main;
        var val = main.startLifetime;

    }
}
