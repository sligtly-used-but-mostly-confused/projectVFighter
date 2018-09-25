using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawnPosition : SpawnPosition {
    public float chanceToSpawn = .5f;

    public override void Spawn()
    {
        float stageMultiplyer = ((float)GameManager.Instance.StageNum / (float)GameManager.Instance.MaxStageNum);
        if (Random.value < 1 - (chanceToSpawn + stageMultiplyer))
        {
            //Destroy(HeldObject);
            for(int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            
            //var obj = Instantiate(HeldObject);
            //obj.transform.position = transform.position;
        }
    }

}
