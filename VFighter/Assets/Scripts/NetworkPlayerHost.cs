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
        StartCoroutine(TryToStart());
    }

    IEnumerator TryToStart()
    {
        var players = PlayerManager.Instance.ConnectedPlayerControllers;
        Player foundPlayer = new Player();
        bool isFound = false;

        foreach(var player in PlayerManager.Instance.ConnectedPlayerControllers)
        {
            if(GetComponent<NetworkIdentity>().playerControllerId == player.NetworkControllerId)
            {
                isFound = true;
                foundPlayer = player;
                break;
            }
        }

        Debug.Log(isFound);

        if (isFound)
        {
            CmdSpawnPlayer(foundPlayer);
        }
        else
        {
            yield return new WaitForSeconds(.1f);
            yield return TryToStart();
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
        //PlayerManager.Instance.ConnectedPlayerControllers.Add(player);
    }

    public void SpawnPlayer(Player player)
    {

    }
}
