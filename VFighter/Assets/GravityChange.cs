using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityChange : MonoBehaviour
{

    public bool gravityChange = true;
   [SerializeField]
    public ParticleSystem gP;
    [SerializeField]
    public GravityObjectRigidBody rd;
    // Use this for initialization;
    bool prevState = true;
    float angle = 180f;
    float pos = -1.5f;
    void Start()
    {
        gP = GetComponent<ParticleSystem>();
        rd = GetComponentInParent<GravityObjectRigidBody>();
    }
    // Update is called once per frame
    void Update()
    {
        float gDir = rd.GravityDirection.x;
        var main = gP.main;
        if (gravityChange != prevState)
        {
            angle *= -1;
            pos *= -1;
            gP.Play();
            this.transform.Rotate(angle, 0.0f, 0.0f);
            this.transform.localPosition = new Vector3(0.0f, pos, 0.0f);

        }
        if(!gP.isPlaying){
            gP.Stop();
        }
        prevState = gravityChange;
        
    }
}
