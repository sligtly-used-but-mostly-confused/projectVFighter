using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

[System.Serializable]
public class PlayerSyncList : SyncListStruct<Player>
{
}


public class PlayerManager : NetworkBehaviour
{
    private static PlayerManager _instance;
    public static PlayerManager Instance { get { return _instance; } }

    //public List<PlayerWrapper> Players = new List<PlayerWrapper>();

    public PlayerSyncList ConnectedPlayerControllers = new PlayerSyncList();
    public List<short> LocalPlayerControllers = new List<short>();
    void Awake()
    {
        if(_instance)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        /*
        foreach(var player in ConnectedPlayerControllers)
        {
            Debug.Log(player.NetworkControllerId + " " + player.NumLives);
        }
        */
    }

    public void AddPlayer(Player player)
    {
        ConnectedPlayerControllers.Add(player);
        //LocalPlayerControllers.Add(player.NetworkControllerId);
        //LevelManager.Instance.SpawnPlayer(player);
    }

    public void ResetPlayers()
    {
        //Players.ForEach(x => x.Player.Reset());
    }
}
