using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSettingsUIController : MonoBehaviour {

    public void SetMasterVol(float v)
    {
        GameRoundSettingsController.Instance.MasterVol = v;
    }

    public void SetMusicVol(float v)
    {
        GameRoundSettingsController.Instance.MusicVol = v;
    }

    public void SetSFXVol(float v)
    {
        GameRoundSettingsController.Instance.SFXVol = v;
    }
}
