using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player {

    [SerializeField]
    public InputDevice PairedInputDevice;
    [SerializeField]
    public Material PlayerMaterial;
    [SerializeField]
    public bool IsKeyboardPlayer = false;
    [SerializeField]
    public int NumWins = 0;
    [SerializeField]
    public int NumDeaths = 0;

    public Player(InputDevice pairedInputDevice, Material playerMaterial, bool isKeyboardPlayer = false, int numWins = 0, int numDeaths = 0)
    {
        PairedInputDevice = pairedInputDevice;
        PlayerMaterial = playerMaterial;
        IsKeyboardPlayer = isKeyboardPlayer;
        NumWins = numWins;
        NumDeaths = numDeaths;
    }
}
