using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReadyIndicatorController : MonoBehaviour {

    public PlayerController AttachedPlayer;

    [SerializeField]
    private Material _readyMaterial;
    [SerializeField]
    private Material _notReadyMaterial;
    
    private void Update()
    {
        if(AttachedPlayer)
        {
            transform.position = AttachedPlayer.transform.position + Vector3.up * AttachedPlayer.transform.localScale.y * 2;
        }

        if(AttachedPlayer && AttachedPlayer.IsReady)
        {
            GetComponent<Renderer>().material = _readyMaterial;
        }
        else
        {
            GetComponent<Renderer>().material = _notReadyMaterial;
        }
    }
}
