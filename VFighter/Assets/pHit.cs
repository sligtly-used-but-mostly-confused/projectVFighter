using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pHit : MonoBehaviour {
    [SerializeField]
    public BoxCollider2D bc;
    [SerializeField]
    public ParticleSystem[] ps;
    ParticleSystem.MainModule main;
    ParticleSystem.EmissionModule em;
  
    // Use this for initialization
    void Start () {
        bc = GetComponent<BoxCollider2D>();
        ps = GetComponentsInChildren<ParticleSystem>();
        main = ps[2].main;
        em = ps[2].emission;
        em.enabled = true;
        main.startSize = 0.0f;
    }
	
	// Update is called once per frame
	

    void OnCollisionEnter2D(Collision2D collision)
    {
        var force = collision.relativeVelocity * collision.otherRigidbody.mass;

        if (force.magnitude > 1.0f)
        {
            // em.enabled = true;
            main.startSize = 0.4f;
            main.simulationSpeed = 1f;
            ps[2].Emit(50);
     
        }
    }

 

}
