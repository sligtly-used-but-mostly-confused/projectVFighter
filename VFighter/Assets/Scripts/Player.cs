using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Player {
    [SerializeField]
    public static short IdCnt;
    [SerializeField]
    public short NetworkControllerId;

    //public InputDevice PairedInputDevice;
    //[SerializeField]
    //public Material PlayerMaterial;
    [SerializeField]
    public bool IsKeyboardPlayer;
    [SerializeField]
    public int NumWins;
    [SerializeField]
    public int NumDeaths;
    [SerializeField]
    public int NumLives;
    [SerializeField]
    public int NumKills;
    [SerializeField]
    public bool ForceSelectInputDeviceByIndex;
    [SerializeField]
    public int InputDeviceIndex;

    public Player(int numLives, bool isKeyboardPlayer = false, int numWins = 0, int numDeaths = 0)
    {
        //PairedInputDevice = pairedInputDevice;
        //PlayerMaterial = playerMaterial;
        IsKeyboardPlayer = isKeyboardPlayer;
        NumWins = numWins;
        NumDeaths = numDeaths;
        NumLives = numLives;
        NumKills = 0;
        NetworkControllerId = 0;
        ForceSelectInputDeviceByIndex = false;
        InputDeviceIndex = 2;

    }

    public void Reset()
    {
        NumWins = 0;
        NumDeaths = 0;
        NumKills = 0;
    }
}
