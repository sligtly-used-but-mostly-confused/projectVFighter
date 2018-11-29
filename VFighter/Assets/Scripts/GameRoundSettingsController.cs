using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameRoundSettingsController : MonoBehaviour {
    public static GameRoundSettingsController Instance;

    public int NumRounds;
    public int NumLivesPerRound;
    public float GameSpeedMin = .5f;
    public float GameSpeedMax = 1;
    public float GameSpeedRate = .05f;
    public int MinPlayers = 2;

    public bool UseTransitions = true;

    private void Awake()
    {
        if(Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
