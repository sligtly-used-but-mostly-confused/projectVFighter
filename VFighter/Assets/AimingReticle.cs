using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AimingReticle : NetworkBehaviour {
    [SyncVar]
    public short Id;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
