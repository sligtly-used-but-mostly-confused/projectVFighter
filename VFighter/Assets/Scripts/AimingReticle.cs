using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingReticle : MonoBehaviour
{
    public short Id;
    public PlayerController PlayerAttachedTo;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if(PlayerAttachedTo)
        {
            //GetComponent<Renderer>().material = PlayerAttachedTo.GetComponent<Renderer>().material;
        }
    }
}
