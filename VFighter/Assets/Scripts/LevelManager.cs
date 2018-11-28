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
    public bool PlayersCanDieInThisLevel = true;
    [SerializeField]
    protected List<PlayerSpawnPosition> _spawnPositions;
    [SerializeField]
    private bool _startNextLevelWinCondition = true;
    private float _gravityScaleGradientStart { get { return GameRoundSettingsController.Instance.GameSpeedMin; } }
    private float _gravityScaleGradientEnd { get { return GameRoundSettingsController.Instance.GameSpeedMax;} }
    private float _gravityScaleGradientRate { get { return GameRoundSettingsController.Instance.GameSpeedRate; } }

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

    public void Start()
    {
        SpawnPlayers();
        var objectSpawns = new List<SpawnPosition>(FindObjectsOfType<ObjectSpawnPosition>());

        foreach (var objectSpawn in objectSpawns)
        {
            objectSpawn.Spawn();
        }

        Players = FindObjectsOfType<PlayerController>().ToList();
        GameManager.Instance.DoneChangingScenes();

        GameManager.Instance.OnLevelChanged();
    }

    public virtual void Update()
    {
        if(!GameManager.Instance.CurrentlyChangingScenes && !GameManager.Instance.DebugScene)
        {
            var alive = Players.Where(x => !x.IsDead);
            if (_startNextLevelWinCondition && alive.Count() <= 1)
            {
                GameManager.Instance.LoadNextStage();
            }
        }
    }

    private void FixedUpdate()
    {
        if(_hasGameStarted)
        {
            GameManager.Instance.TimeScale += _gravityScaleGradientRate * Time.fixedDeltaTime;
            GameManager.Instance.TimeScale = Mathf.Clamp(GameManager.Instance.TimeScale, 0, _gravityScaleGradientEnd);
        }
    }

    public void StartGame()
    {
        GameManager.Instance.TimeScale = _gravityScaleGradientStart;
        Debug.Log(GameManager.Instance.TimeScale);
        _hasGameStarted = true;
    }

    protected virtual void SpawnPlayers()
    {
        var players = FindObjectsOfType<PlayerController>().ToList();
        List<PlayerSpawnPosition> spawnPositionsCopy = new List<PlayerSpawnPosition>(_spawnPositions);
        players.ForEach(x => SpawnPlayer(x, spawnPositionsCopy));
    }

    public virtual void SpawnPlayer(PlayerController player)
    {
        SpawnPlayer(player, new List<PlayerSpawnPosition>(_spawnPositions));
    }

    //makes it so that once a spawn position is used another player can not spawn there
    //used on the controller select screen
    public virtual void SpawnPlayerDestructive(PlayerController player)
    {
        SpawnPlayer(player, _spawnPositions);
    }

    public virtual void SpawnPlayer(PlayerController player, List<PlayerSpawnPosition> spawnPositions)
    {
        if(player.ControlledPlayer.NumLives - player.ControlledPlayer.NumDeaths <= 0)
        {
            player.InitializeForStartLevel(JailTransform.position, true);
            return;
        }

        int index = (int)(Random.value * (spawnPositions.Count - 1));
        PlayerSpawnPosition position = spawnPositions[index];
        spawnPositions.RemoveAt(index);
        player.InitializeForStartLevel(position.gameObject.transform.position, false);
        position.Spawn(player);
    }
}
