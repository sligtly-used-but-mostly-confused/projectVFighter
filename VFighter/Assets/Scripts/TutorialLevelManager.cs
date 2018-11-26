using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialLevelManager : LevelManager {

    public MappedIconSprite MappedIcon;

    public override void SpawnPlayer(PlayerController player)
    {
        base.SpawnPlayer(player);
        MappedIcon.controller = player.InputDevice;
    }
}
