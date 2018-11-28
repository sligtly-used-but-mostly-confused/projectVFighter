using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndScoreScreenPlayerSpawner : PlayerSpawnPosition {

    public MappedIconSprite MappedIcon;
    public TextMeshPro PlayerName;
    public TransitionController TController;


    private PlayerController _player;
    private bool _prevReadyState = false;
    public override void Spawn(PlayerController player)
    {
        base.Spawn(player);
        _player = player;
        MappedIcon.controller = player.InputDevice;
        PlayerName.text = $"P{player.PlayerId}";
        PlayerName.color = player.GetComponent<CharacterSelectController>().CurrentPlayerColor;
    }

    private void Update()
    {
        if(_player && _player.IsReady && !_prevReadyState)
        {
            if(TController)
            {
                TController.StartTransition(() => { });
            }
        }

        if(_player)
            _prevReadyState = _player.IsReady;
    }
}
