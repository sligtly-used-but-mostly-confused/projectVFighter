using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialEvent : MonoBehaviour {
    public UnityEvent OnFinish;
    [SerializeField]
    protected PlayerController AttachedPlayer;
    public PlayerSpawnPosition PlayerSpawn;

    private void Awake()
    {
        PlayerSpawn.OnSpawn += OnPlayerSpawnned;
    }

    private void OnPlayerSpawnned(GameObject player)
    {
        AttachedPlayer = player.GetComponent<PlayerController>();
    }
}
