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
        //if (player.IsKeyboardPlayer)
        //{
            playerObj = Instantiate(_keyboardPlayerControllerPrefab);
        //}
        //else
        //{
            //playerObj = Instantiate(_gamepadPlayerControllerPrefab);
        //}
        
        //playerObj.GetComponent<PlayerController>().Init(player, transform);
        playerObj.GetComponent<PlayerController>().PlayerId = playerId;
        playerObj.name = "controller "+GetComponent<NetworkIdentity>().playerControllerId;
        controllerId = GetComponent<NetworkIdentity>().playerControllerId;
        NetworkServer.Spawn(playerObj);

        //PlayerManager.Instance.ConnectedPlayerControllers.Add(player);
    }

    public void SpawnPlayer(Player player)
    {

    }
}
