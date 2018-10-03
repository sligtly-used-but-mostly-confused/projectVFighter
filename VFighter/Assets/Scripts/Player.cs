using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Player {
    [SerializeField]
    public short NetworkControllerId;
    [SerializeField]
    public int NumWins;
    [SerializeField]
    public int NumDeaths;
    [SerializeField]
    public int NumLives;
    [SerializeField]
    public int NumKills;

    public Player(int numLives, int numWins = 0, int numDeaths = 0)
    {
        NumWins = numWins;
        NumDeaths = numDeaths;
        NumLives = numLives;
        NumKills = 0;
        NetworkControllerId = 0;
    }

    public void Reset()
    {
        NumWins = 0;
        NumDeaths = 0;
        NumKills = 0;
    }
}
