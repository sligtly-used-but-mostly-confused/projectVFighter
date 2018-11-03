using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPosition : SpawnPosition {

    public GameObject prompt;

    public virtual void Spawn(PlayerController player)
    {
        if(prompt)
        {
            Destroy(prompt);
        }
    }
}
