using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPosition : MonoBehaviour {

    public delegate void OnSpawnDelegate(GameObject spawnnedObject);
    public OnSpawnDelegate OnSpawn;

    private void Awake()
    {
        OnSpawn += (x) => { };
    }

    public virtual void Spawn()
    {

    }
}
