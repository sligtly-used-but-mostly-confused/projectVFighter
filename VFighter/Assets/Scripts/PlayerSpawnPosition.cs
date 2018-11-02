using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPosition : SpawnPosition {

    public GameObject prompt;

    public override void Spawn()
    {
        if(prompt)
        {
            Destroy(prompt);
        }
    }
}
