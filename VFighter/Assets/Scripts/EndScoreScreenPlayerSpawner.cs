using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndScoreScreenPlayerSpawner : PlayerSpawnPosition {

    public MappedIconSprite MappedIcon;
    public MappedIconSprite FlipCardMappedIcon;
    public TextMeshPro PlayerName;
    public TransitionController TController;
    public PlayerStatisticsUIController StatsController;
    public GameObject ReadyPanel;
    public Transform PlayerSpawnPosition;

    private PlayerController _player;
    private bool _prevReadyState = false;
    public override void Spawn(PlayerController player)
    {
        base.Spawn(player);
        _player = player;
        MappedIcon.controller = player.InputDevice;
        FlipCardMappedIcon.controller = player.InputDevice;
        PlayerName.text = $"P{player.PlayerId}";
        PlayerName.color = player.GetComponent<CharacterSelectController>().CurrentPlayerColor;
    }

    private void Update()
    {
        if(_player && _player.InputDevice.GetButtonDown(MappedButton.FlipToStatsCard))
        {
            if (TController)
            {
                StatsController.gameObject.SetActive(!StatsController.gameObject.activeInHierarchy);

                if (StatsController.gameObject.activeInHierarchy)
                {
                    StatsController.Init(_player.ControlledPlayer);
                }
            }
        }

        if (_player && _player.InputDevice.GetButtonDown(MappedButton.Ready))
        {
            if (TController)
            {
                ReadyPanel.SetActive(!ReadyPanel.activeInHierarchy);
            }
        }
    }
}
