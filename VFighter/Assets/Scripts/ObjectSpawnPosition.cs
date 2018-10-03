using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ObjectSpawnPosition : SpawnPosition {
    public float chanceToSpawn = .5f;

    public override void Spawn()
    {
        float stageMultiplyer = GameManager.Instance.ProgressionThroughGame;
        if (Random.value > chanceToSpawn * stageMultiplyer)
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                NetworkServer.Destroy(transform.GetChild(i).gameObject);
            }
        }
    }

}
