using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkPlayerHost : NetworkBehaviour{
    [SerializeField]
    public static short _idCnt;

    [SerializeField]
    private GameObject _gamepadPlayerControllerPrefab;
    [SerializeField]
    private GameObject _keyboardPlayerControllerPrefab;

    [SyncVar]
    public short playerId;

    [SyncVar]
    public short controllerId;

    public bool IsLocal = false;

    public override void OnStartLocalPlayer()
    {
        IsLocal = true;
        CmdSpawnPlayer();
    }

    [Command]
    public void CmdSpawnPlayer()
    {
        playerId = _idCnt++;
        GameObject playerObj;
        playerObj = Instantiate(_keyboardPlayerControllerPrefab);
        playerObj.name = "controller "+GetComponent<NetworkIdentity>().playerControllerId;
        controllerId = GetComponent<NetworkIdentity>().playerControllerId;
        NetworkServer.Spawn(playerObj);
    }

    public void SpawnPlayer(Player player)
    {

    }
}
