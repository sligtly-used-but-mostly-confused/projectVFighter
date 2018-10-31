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
    [SerializeField]
    public int NumOverallKills;
    [SerializeField]
    public int NumOverallDeaths;
    [SerializeField]
    public int NumRoundWins;

    public Player(int numLives, int numWins = 0, int numDeaths = 0)
    {
        NumWins = numWins;
        NumDeaths = numDeaths;
        NumLives = numLives;
        NumKills = 0;
        NetworkControllerId = 0;
        NumOverallKills = 0;
        NumOverallDeaths = 0;
        NumRoundWins = 0;
    }

    public void Reset()
    {
        NumWins = 0;
        NumDeaths = 0;
        NumKills = 0;
    }

    public void ResetForNewGame()
    {
        NumWins = 0;
        NumDeaths = 0;
        NumKills = 0;
        NumOverallKills = 0;
        NumOverallDeaths = 0;
        NumRoundWins = 0;
    }
}
