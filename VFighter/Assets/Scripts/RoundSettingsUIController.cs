using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundSettingsUIController : MonoBehaviour {
    public void SetNumRounds(TMP_Dropdown dropdown)
    {
        GameRoundSettingsController.Instance.NumRounds = Int32.Parse(dropdown.options[dropdown.value].text);
        LevelSelectManager.Instance.RefreshRoundSettings();
    }

    public void SetNumLivesPerRound(TMP_InputField text)
    {
        GameRoundSettingsController.Instance.NumLivesPerRound = Int32.Parse(text.text);
        LevelSelectManager.Instance.RefreshRoundSettings();
    }
}
