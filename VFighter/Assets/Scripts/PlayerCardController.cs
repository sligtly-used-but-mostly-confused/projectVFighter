using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCardController : MonoBehaviour {

    [SerializeField]
    private Text _livesText;
    [SerializeField]
    private Text _killsText;

    [SerializeField]
    private Text _overallDeathsText;
    [SerializeField]
    private Text _overallKillsText;
    [SerializeField]
    private Text _overallWinsText;

    private PlayerController _attachedPlayer;

    public void AttachToPlayer(PlayerController player)
    {
        _attachedPlayer = player;
        _livesText.text = "" + (player.ControlledPlayer.NumLives - player.ControlledPlayer.NumDeaths);
        _killsText.text = "" + player.ControlledPlayer.NumKills;

        _overallDeathsText.text = "" + player.ControlledPlayer.NumOverallDeaths;
        _overallKillsText.text = "" + player.ControlledPlayer.NumOverallKills;
        _overallWinsText.text = "" + player.ControlledPlayer.NumStageWins;
    }
}
