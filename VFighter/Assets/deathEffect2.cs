using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deathEffect2 : MonoBehaviour {
    [SerializeField]
    public BoxCollider2D bc;
    [SerializeField]
    public ParticleSystem ps;

    // Use this for initialization
    void Start () {
        bc = GetComponentInParent<BoxCollider2D>();
        ps = GetComponent<ParticleSystem>();
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    private void OnCollisionEnter(Collision collision)
    {
        ps.Play();
    }
}
