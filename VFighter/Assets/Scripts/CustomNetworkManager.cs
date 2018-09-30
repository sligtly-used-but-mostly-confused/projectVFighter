using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomNetworkManager : NetworkManager {

    public static CustomNetworkManager Instance = null;

    private void Awake()
    {
        Instance = this;
    }
}

