using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoundSettingsUIController : MonoBehaviour {

    public TMP_Dropdown NumRoundsDropDown;
    public TMP_InputField LivesPerRoundInput;
    public Toggle UseTransitionsToggle;
    public Slider MasterVolSlider;
    public Slider MusicVolSlider;
    public Slider SFXVolSlider;

    public GameObject SettingsContainer;
    public bool IsSettingMenuDisplayed { get { return SettingsContainer.activeSelf; } }
    public void ToggleSettingsMenu()
    {
        SettingsContainer.SetActive(!SettingsContainer.activeSelf);
        OnOpen();
    }

    public void OnOpen()
    {
        Debug.Log(UseTransitionsToggle);
        UseTransitionsToggle.isOn = GameRoundSettingsController.Instance.UseTransitions;
        LivesPerRoundInput.text = GameRoundSettingsController.Instance.NumLivesPerRound.ToString();

        int dropdownOption = 0;
        
        for(int i = 0; i < NumRoundsDropDown.options.Count; i++)
        {
            if(Int32.Parse(NumRoundsDropDown.options[i].text) == GameRoundSettingsController.Instance.NumRounds)
            {
                dropdownOption = i;
            }
        }

        NumRoundsDropDown.value = dropdownOption;
        MasterVolSlider.value = GameRoundSettingsController.Instance.MasterVol;
        MusicVolSlider.value = GameRoundSettingsController.Instance.MusicVol;
        SFXVolSlider.value = GameRoundSettingsController.Instance.SFXVol;
    }

    public void SetNumRounds(TMP_Dropdown dropdown)
    {
        GameRoundSettingsController.Instance.NumRounds = Int32.Parse(dropdown.options[dropdown.value].text);
        if(LevelSelectManager.Instance)
            LevelSelectManager.Instance.RefreshRoundSettings();
    }

    public void SetNumLivesPerRound(TMP_InputField text)
    {
        GameRoundSettingsController.Instance.NumLivesPerRound = Int32.Parse(text.text);
        LevelSelectManager.Instance.RefreshRoundSettings();
    }

    public void SetUseTransitions()
    {
        GameRoundSettingsController.Instance.UseTransitions = UseTransitionsToggle.isOn;
        LevelSelectManager.Instance.RefreshRoundSettings();
    }
}
