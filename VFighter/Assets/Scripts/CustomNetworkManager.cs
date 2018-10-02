﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public class CustomNetworkManager : NetworkManager {

    public static CustomNetworkManager Instance = null;
    [SerializeField]
    public List<Material> _playerMaterials = new List<Material>();
    private List<short> _playerMaterialIndexes = new List<short>();

    private void Awake()
    {
        Instance = this;
        for (short i = 0; i < _playerMaterials.Count; i++)
        {
            _playerMaterialIndexes.Add(i);
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        
        //Material mat = _playerMaterials[matIndex];
        //_playerMaterials.RemoveAt(matIndex);
        //playerPrefab.GetComponent<Renderer>().material = mat;

        GameObject player = (GameObject)Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

        int matIndexIndex = (int)(UnityEngine.Random.value * (_playerMaterialIndexes.Count - 1));
        short matIndex = _playerMaterialIndexes[matIndexIndex];
        _playerMaterialIndexes.RemoveAt(matIndexIndex);
        player.GetComponent<PlayerController>().MaterialId = matIndex;

        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }
}

