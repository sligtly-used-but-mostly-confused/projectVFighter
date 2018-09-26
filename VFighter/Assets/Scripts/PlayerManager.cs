using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager _instance;
    public static PlayerManager Instance { get { return _instance; } }

    public List<Player> Players = new List<Player>();

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

    public void AddPlayer(Player player)
    {
        Players.Add(player);
        LevelManager.Instance.SpawnPlayer(player);
    }

    public void ResetPlayers()
    {
        Players.ForEach(x => { x.NumDeaths = 0; x.NumWins = 0; });
    }
}
