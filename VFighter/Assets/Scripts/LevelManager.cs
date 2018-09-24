﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    private static LevelManager _instance;
    public static LevelManager Instance { get { return _instance; } }

    public List<PlayerController> Players;

    [SerializeField]
    private List<SpawnPosition> _spawnPositions;
    [SerializeField]
    private GameObject GamepadPlayerControllerPrefab;

    void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        var alive = Players.Where(x => !x.IsDead);
        if (alive.Count() <= 1)
        {
            //Debug.Log(alive.First()?.name + " won");
            ResetLevel();
        }
    }

    private void Init()
    {
        _spawnPositions = new List<SpawnPosition>(FindObjectsOfType<PlayerSpawnPosition>());

        foreach (var player in PlayerManager.Instance.Players)
        {
            int index = (int)(Random.value * (_spawnPositions.Count - 1));
            SpawnPosition position = _spawnPositions[index];
            _spawnPositions.RemoveAt(index);
            var playerObj = Instantiate(GamepadPlayerControllerPrefab);
            playerObj.GetComponent<PlayerController>().Init(player, position);

            Players.Add(playerObj.GetComponent<PlayerController>());
        }
    }

    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
