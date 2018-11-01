using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHit : MonoBehaviour {
    [SerializeField]
    public BoxCollider2D rd;
    [SerializeField]
    public ParticleSystem[] ps;
   
    // Use this for initialization
    void Start () {
        rd = GetComponent<BoxCollider2D>();
        ps = GetComponentsInChildren<ParticleSystem>();
    }

    void Update()
    {
        if (!ps[1].isPlaying) {
            ps[1].Stop();
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
   
        var force = collision.relativeVelocity * collision.otherRigidbody.mass;
       
            if (force.magnitude > 5.0f)
            {
            ps[1].Play();
              
            }

    }
}
