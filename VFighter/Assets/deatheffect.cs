using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deatheffect : MonoBehaviour {

    public bool isDead = false;
    [SerializeField]
    public ParticleSystem pulse;
    [SerializeField]
    public KeyboardPlayerController kp;
    [SerializeField]
    public GameObject DeathPrefab;
    [SerializeField]
    public BoxCollider2D bc;
    private GameObject cloneObject;
    private Transform location;
    // Use this for initialization;
    void Start()
    {
        kp = GetComponent<KeyboardPlayerController>();
        bc = GetComponent<BoxCollider2D>();
    }
  
    // Update is called once per frame
    void Update()
    {
        
         
        if(pulse != null)
        {
            if (!pulse.isPlaying)
            {
                Destroy(cloneObject);
            }
        }
      
    }
  void OnCollisionEnter2D(Collision2D other)
    {
       
        if(other.collider.tag == "projectile")
        {
            cloneObject = Instantiate(DeathPrefab);
            cloneObject.transform.position = other.transform.position;
            pulse = cloneObject.GetComponent<ParticleSystem>();
            pulse.Play();
        }
     
    }

}
