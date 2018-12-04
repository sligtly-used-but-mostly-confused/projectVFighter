using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCardController : MonoBehaviour {
    [SerializeField]
    private TextMeshProUGUI _playerIdText;
    [SerializeField]
    private TextMeshProUGUI _overallDeathsText;
    [SerializeField]
    private TextMeshProUGUI _overallKillsText;
    [SerializeField]
    private TextMeshProUGUI _overallWinsText;

    private PlayerController _attachedPlayer;

    public void AttachToPlayer(PlayerController player)
    {
        _attachedPlayer = player;
        _overallDeathsText.text = "" + player.ControlledPlayer.NumOverallDeaths;
        _overallKillsText.text = "" + player.ControlledPlayer.NumOverallKills;
        _overallWinsText.text = "" + player.ControlledPlayer.NumStageWins;
        _playerIdText.text = "P" + player.PlayerId;
        _playerIdText.color = player.GetComponent<CharacterSelectController>().CurrentPlayerColor;
    }
}
