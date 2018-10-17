using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AimingReticle : NetworkBehaviour {
    [SyncVar]
    public short Id;
    [SyncVar]
    public NetworkInstanceId PlayerAttachedTo;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        var player = ClientScene.FindLocalObject(PlayerAttachedTo);

        if(player)
        {
            GetComponent<Renderer>().material = player.GetComponent<Renderer>().material;
        }
    }
}
