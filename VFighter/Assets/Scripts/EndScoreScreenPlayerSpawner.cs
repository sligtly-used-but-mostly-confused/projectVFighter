using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndScoreScreenPlayerSpawner : PlayerSpawnPosition {

    public MappedIconSprite MappedIcon;
    public TextMeshPro PlayerName;

    public override void Spawn(PlayerController player)
    {
        base.Spawn(player);
        MappedIcon.controller = player.InputDevice;
        PlayerName.text = $"P{player.PlayerId}";
        PlayerName.color = player.GetComponent<CharacterSelectController>().CurrentPlayerColor;
    }
}
