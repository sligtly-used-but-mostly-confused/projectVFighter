using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawnPosition : SpawnPosition {
    public float chanceToSpawn = .5f;

    public override void Spawn()
    {
        float stageMultiplyer = (float)GameManager.Instance.StageNum / (float)GameManager.Instance.MaxStageNum;
        if (Random.value > chanceToSpawn * stageMultiplyer)
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }

}
