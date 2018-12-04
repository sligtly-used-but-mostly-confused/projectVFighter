using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReadyIndicatorController : MonoBehaviour {

    public PlayerController AttachedPlayer;

    [SerializeField]
    private Material _readyMaterial;
    [SerializeField]
    private Material _notReadyMaterial;
    [SerializeField]
    private Vector3 _offset;
    private void Update()
    {
        if(AttachedPlayer)
        {
            transform.position = AttachedPlayer.transform.position + _offset;
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
