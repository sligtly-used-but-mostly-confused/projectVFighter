using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMappedIconSprite : MappedIconSprite {
    public PlayerSpawnPosition PlayerSpawn;

    private void Awake()
    {
        PlayerSpawn.OnSpawn += OnPlayerSpawnned;
    }

    private void OnPlayerSpawnned(GameObject player)
    {
        controller = player.GetComponent<PlayerController>().InputDevice;
    }
}
