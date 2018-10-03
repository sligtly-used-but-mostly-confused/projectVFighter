using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Linq;

public class LevelManager : NetworkBehaviour
{
    private static LevelManager _instance;
    public static LevelManager Instance { get { return _instance; } }

    public List<PlayerController> Players;
    public bool PlayersCanDieInThisLevel = true;
    [SerializeField]
    private List<SpawnPosition> _spawnPositions;
    [SerializeField]
    private bool _startNextLevelWinCondition = true;
    [SerializeField]
    private float _gravityScaleGradientStart = .5f;
    [SerializeField]
    private float _gravityScaleGradientEnd = 1f;
    [SerializeField]
    private float _gravityScaleGradientRate = .1f;
    [SerializeField]
    private bool _hasGameStarted = false;

    [SerializeField]
    public Transform JailTransform;

    void Awake()
    {
        if(_instance)
        {
            Destroy(_instance.gameObject);
        }

        _instance = this;
    }

    public override void OnStartServer()
    {
        StartCoroutine(Init());
        Players = FindObjectsOfType<PlayerController>().ToList();
        GameManager.Instance.DoneChangingScenes();
    }

    private void Update()
    {
        if(isServer && !GameManager.Instance.CurrentlyChangingScenes)
        {
            var alive = Players.Where(x => !x.IsDead);
            if (_startNextLevelWinCondition && alive.Count() <= 1)
            {
                if (alive.Count() > 0)
                {
                    alive.First().ControlledPlayer.NumWins++;
                    alive.First().SetDirtyBit(0xFFFFFFFF);
                }

                GameManager.Instance.LoadNextStage();
            }
        }
    }

    private void FixedUpdate()
    {
        if(_hasGameStarted && isServer)
        {
            GameManager.Instance.TimeScale += _gravityScaleGradientRate * Time.fixedDeltaTime;
            GameManager.Instance.TimeScale = Mathf.Clamp(GameManager.Instance.TimeScale, _gravityScaleGradientStart, _gravityScaleGradientEnd);
        }
    }

    public void StartGame()
    {
        GameManager.Instance.TimeScale = _gravityScaleGradientStart;
        _hasGameStarted = true;
    }

    public void SpawnPlayer(PlayerController player)
    {
        int index = (int)(Random.value * (_spawnPositions.Count - 1));
        SpawnPosition position = _spawnPositions[index];
        _spawnPositions.RemoveAt(index);
        player.InitializeForStartLevel(position.gameObject);
    }

    private IEnumerator Init()
    {
        yield return new WaitForSeconds(1f);
        _spawnPositions = new List<SpawnPosition>(FindObjectsOfType<PlayerSpawnPosition>());

        var players = FindObjectsOfType<PlayerController>().ToList();
        players.ForEach(x => SpawnPlayer(x));
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
