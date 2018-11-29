using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pHit : MonoBehaviour {
    [SerializeField]
    public BoxCollider2D bc;
    [SerializeField]
    public ParticleSystem[] ps;
    ParticleSystem.MainModule main;
    // Use this for initialization
    void Start () {
        bc = GetComponent<BoxCollider2D>();
        ps = GetComponentsInChildren<ParticleSystem>();
        main = ps[3].main;
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("hit");
    }

}
