﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player {

    [SerializeField]
    public int InputDeviceIndex = 0;
    [SerializeField]
    public Material PlayerMaterial;
    [SerializeField]
    public bool IsKeyboardPlayer = false;
    [SerializeField]
    public int NumWins = 0;
    [SerializeField]
    public int NumDeaths = 0;


    public Player(int inputDeviceIndex)
    {
        this.InputDeviceIndex = inputDeviceIndex;
    }
}