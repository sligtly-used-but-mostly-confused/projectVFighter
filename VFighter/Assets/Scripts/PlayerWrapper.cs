using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerWrapper {
    [SerializeField]
    public Player Player;


    public static short IdCnt { get { return Player.IdCnt; } set { Player.IdCnt = value; } }
    public short NetworkControllerId { get { return this.Player.NetworkControllerId; } set { this.Player.NetworkControllerId = value; } }
    public bool IsKeyboardPlayer { get { return this.Player.IsKeyboardPlayer; } set { this.Player.IsKeyboardPlayer = value; } }
    public int NumWins { get { return this.Player.NumWins; } set { this.Player.NumWins = value; } }
    public int NumDeaths { get { return this.Player.NumDeaths; } set { this.Player.NumDeaths = value; } }
    public int NumLives { get { return this.Player.NumLives; } set { this.Player.NumLives = value; } }
    public int NumKills { get { return this.Player.NumKills; } set { this.Player.NumKills = value; } }
    public bool ForceSelectInputDeviceByIndex { get { return this.Player.ForceSelectInputDeviceByIndex; } set { this.Player.ForceSelectInputDeviceByIndex = value; } }
    public int InputDeviceIndex { get { return this.Player.InputDeviceIndex; } set { this.Player.InputDeviceIndex = value; } }
}
