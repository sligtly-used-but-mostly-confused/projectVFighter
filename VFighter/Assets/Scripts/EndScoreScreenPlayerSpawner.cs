using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScoreScreenPlayerSpawner : PlayerSpawnPosition {

    public MappedIconSprite MappedIcon;

    public override void Spawn(PlayerController player)
    {
        base.Spawn(player);
        MappedIcon.controller = player.InputDevice;
    }
}
