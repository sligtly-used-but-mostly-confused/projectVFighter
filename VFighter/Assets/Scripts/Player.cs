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
    [SerializeField]
    public int NumLives = 0;
    [SerializeField]
    public bool ForceSelectInputDeviceByIndex = false;
    [SerializeField]
    public int InputDeviceIndex = 2;

    public Player(InputDevice pairedInputDevice, Material playerMaterial, int numLives, bool isKeyboardPlayer = false, int numWins = 0, int numDeaths = 0)
    {
        PairedInputDevice = pairedInputDevice;
        PlayerMaterial = playerMaterial;
        IsKeyboardPlayer = isKeyboardPlayer;
        NumWins = numWins;
        NumDeaths = numDeaths;
        NumLives = numLives;
    }
}
