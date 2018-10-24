using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashControl : MonoBehaviour {
    [SerializeField]
    private Rigidbody2D rd;
    [SerializeField]
    private Transform tm;
    [SerializeField]
    private DashEffect de;
    Vector2 velVec;
    bool flag = true;
    [SerializeField]
    private KeyboardPlayerController kPC;


    // Use this for initialization
    // Update is called once per frame
    private void Start()
    {
        rd = GetComponentInParent<Rigidbody2D>();
        tm = GetComponentInParent<Transform>();
        de = GetComponent<DashEffect>();
        kPC = GetComponentInParent<KeyboardPlayerController>();
    }
    void Update () {

        if (de.dashEffectOn() && flag)
        {
            float dir = Mathf.Atan2(rd.velocity.y, rd.velocity.x);
          
            tm.Rotate(0.0f, 0.0f, dir * Mathf.Rad2Deg);
            flag = false;
        }
        else if (!de.dashEffectOn() && !flag){
            flag = true;
        }
        print(kPC.DashDirection());
    }
}
