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

    void Start()
    {
        kp = GetComponent<KeyboardPlayerController>();
        bc = GetComponent<BoxCollider2D>();
    }
  
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

    public void PlayDeathEffect()
    {
        cloneObject = Instantiate(DeathPrefab);
        cloneObject.transform.position = transform.position;
        pulse = cloneObject.GetComponent<ParticleSystem>();
        pulse.Play();
    }
}
