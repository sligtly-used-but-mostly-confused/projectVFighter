using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameRoundSettingsController : MonoBehaviour {
    public static GameRoundSettingsController Instance;

    public int NumRounds;
    public int NumLivesPerRound;
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

    public void SetNumRounds(TMP_Dropdown dropdown)
    {
        NumRounds = Int32.Parse(dropdown.options[dropdown.value].text);
    }

    public void SetNumLivesPerRound(TMP_InputField text)
    {
        NumLivesPerRound = Int32.Parse(text.text);
    }
}
