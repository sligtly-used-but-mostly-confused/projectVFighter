using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Player {
    [SerializeField]
    public short NetworkControllerId;

    //kept track on a per stage basis
    [SerializeField]
    public int NumDeaths;
    [SerializeField]
    public int NumLives;
    [SerializeField]
    public int NumKills;

    //kept track over the couse of the entire game
    [SerializeField]
    public int NumOverallKills;
    [SerializeField]
    public int NumOverallDeaths;
    [SerializeField]
    public int NumStageWins;

    public Player(int numLives, int numDeaths = 0)
    {
        NumDeaths = numDeaths;
        NumLives = numLives;
        NumKills = 0;
        NetworkControllerId = 0;
        NumOverallKills = 0;
        NumOverallDeaths = 0;
        NumStageWins = 0;
    }

    public void Reset()
    {
        NumDeaths = 0;
        NumKills = 0;
    }

    public void ResetForNewGame()
    {
        NumDeaths = 0;
        NumKills = 0;
        NumOverallKills = 0;
        NumOverallDeaths = 0;
        NumStageWins = 0;
    }
}
