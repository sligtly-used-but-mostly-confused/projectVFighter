using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashControl : MonoBehaviour {
    [SerializeField]
    private Rigidbody2D rb;
    [SerializeField]
    private Transform tm;
    [SerializeField]
    private DashEffect de;
    Vector2 velVec;

    // Use this for initialization
    // Update is called once per frame
    private void Start()
    {
        rb = GetComponentInParent<Rigidbody2D>();
        tm = GetComponentInParent<Transform>();
        de = GetComponent<DashEffect>();
    }
    void Update () {

        if (de.dashEffectOn())
        {
         
            Vector2 dir = rb.velocity;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 180;
            if(angle < 10)
            {
                de.dashOn = false;
            }
            else
            {
                tm.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }
     
    }
}
