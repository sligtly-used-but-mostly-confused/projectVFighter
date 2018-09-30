using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkPlayerHost : NetworkBehaviour{
    [SerializeField]
    public static int _idCnt;

    [SerializeField]
    private GameObject _gamepadPlayerControllerPrefab;
    [SerializeField]
    private GameObject _keyboardPlayerControllerPrefab;

    public override void OnStartLocalPlayer()
    {
        //GetComponent<NetworkIdentity>().playerControllerId
        var players = PlayerManager.Instance.Players.FindAll(x => x.NetworkControllerId == GetComponent<NetworkIdentity>().playerControllerId);
        if (players.Count > 0)
        {
            CmdSpawnPlayer(players[0]);
        }
    }

    [Command]
    public void CmdSpawnPlayer(Player player)
    {
        GameObject playerObj;
        if (player.IsKeyboardPlayer)
        {
            playerObj = Instantiate(_keyboardPlayerControllerPrefab);
        }
        else
        {
            playerObj = Instantiate(_gamepadPlayerControllerPrefab);
        }

        playerObj.GetComponent<PlayerController>().Init(player, transform);
        NetworkServer.Spawn(playerObj);
    }

    public void SpawnPlayer(Player player)
    {

    }
}
