using System.Collections;
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
    private GameObject _gamepadPlayerControllerPrefab;
    [SerializeField]
    private GameObject _keyboardPlayerControllerPrefab;
    [SerializeField]
    private bool _startNextLevelWinCondition = true;
    void Awake()
    {
        if(_instance)
        {
            Destroy(_instance.gameObject);
        }

        _instance = this;
    }

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        var alive = Players.Where(x => !x.IsDead);
        if (_startNextLevelWinCondition && alive.Count() <= 1)
        {
            if(alive.Count() > 0)
            {
                alive.First().ControlledPlayer.NumWins++;
            }

            //Players.Where(x => x.IsDead).ToList().ForEach(x => x.ControlledPlayer.NumDeaths++);
            GameManager.Instance.LoadNextStage();
        }
    }

    public void SpawnPlayer(Player player)
    {
        int index = (int)(Random.value * (_spawnPositions.Count - 1));
        SpawnPosition position = _spawnPositions[index];
        _spawnPositions.RemoveAt(index);

        GameObject playerObj = null;

        if(player.IsKeyboardPlayer)
        {
            playerObj = Instantiate(_keyboardPlayerControllerPrefab);
        }
        else
        {
            playerObj = Instantiate(_gamepadPlayerControllerPrefab);
        }
            

        playerObj.GetComponent<PlayerController>().Init(player, position.transform);
        Players.Add(playerObj.GetComponent<PlayerController>());
    }

    private void Init()
    {
        _spawnPositions = new List<SpawnPosition>(FindObjectsOfType<PlayerSpawnPosition>());

        foreach (var player in PlayerManager.Instance.Players)
        {
            SpawnPlayer(player);
        }

        var objectSpawns = new List<SpawnPosition>(FindObjectsOfType<ObjectSpawnPosition>());

        foreach(var objectSpawn in objectSpawns)
        {
            objectSpawn.Spawn();
        }
    }

    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
