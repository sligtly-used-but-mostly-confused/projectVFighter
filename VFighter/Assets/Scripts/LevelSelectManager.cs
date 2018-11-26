using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;

public class LevelSelectManager : MonoBehaviour
{
    public static LevelSelectManager Instance;

    [System.Serializable]
    public struct levelData{
        public string levelName;
        public Material previewMaterial;
    }

    public List<levelData> levels;
    public GameObject platform;
    public GameObject levelZone;
    public float areaWidth;
    public TextMeshProUGUI timer;
    public int selectTime;
    public bool IsTimerStarted { get { return _timerCoroutine != null; } }
    public int numLivesPerPlayer;
    public AudioClip countdown;
    public AudioClip countdownFinal;

    private List<LevelZoneController> zones = new List<LevelZoneController>();
    private Coroutine _timerCoroutine;

    [SerializeField]
    private GameObject _playerReadyIndicatorPrefab;
    
    [SerializeField]
    private bool _isWaitingForReady = true;
    [SerializeField]
    private int timeRemaining;
    [SerializeField]
    private int _minPlayers = 2;
    [SerializeField]
    private int _numRounds = 1;

    private void Awake()
    {
        if(Instance)
        {
            Destroy(gameObject);
        }

        Instance = this;
    }

    void Start()
    {
        SpawnLevelPlatforms();
        timeRemaining = selectTime;

        //the first time we load the game this list will be empty, but after that it will have players
        FindObjectsOfType<PlayerController>().ToList().ForEach(x =>
        {
            var indicator = Instantiate(_playerReadyIndicatorPrefab);
            indicator.GetComponent<PlayerReadyIndicatorController>().AttachedPlayer = x;

            x.IsReady = false;
        });

        RefreshRoundSettings();
    }

    public void RefreshRoundSettings()
    {
        _numRounds = GameRoundSettingsController.Instance.NumRounds;
        numLivesPerPlayer = GameRoundSettingsController.Instance.NumLivesPerRound;
        var players = FindObjectsOfType<PlayerController>().ToList();
        players.ForEach(x => x.ControlledPlayer.NumLives = numLivesPerPlayer);
    }

    private void Update()
    {
        if(timer != null)
            timer.text = timeRemaining.ToString();

        //if all players are ready start the timer
        if (CheckForAllPlayersReady() && !Instance.IsTimerStarted)
        {
            Instance.StartTimer();
        }

        //if all players are not ready stop timer
        if (!CheckForAllPlayersReady() && Instance.IsTimerStarted)
        {
            Instance.StopTimer();
        }
    }

    public List<string> LeadingLevels(){
        var copy = new List<LevelZoneController>(zones);
        copy.Sort((x,y) => { return x.playersInside.CompareTo(y.playersInside); });
        if(copy.Last().playersInside == 0)
        {
            return copy.Select(x => x.levelName).Take(_numRounds).ToList();
        }
        var sorted = copy.Select(x => x.levelName).Reverse();
        return sorted.Take(_numRounds).ToList();
    }

    public void StartTimer()
    {
        StopTimer();
        timeRemaining = selectTime;
        _timerCoroutine = StartCoroutine(CountDown());
    }

    public void StopTimer()
    {
        if(_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
        }

        _timerCoroutine = null;
    }
    
    private bool CheckForAllPlayersReady()
    {
        bool enoughPlayersToStart = FindObjectsOfType<PlayerController>().Count() >= _minPlayers;
        return enoughPlayersToStart && FindObjectsOfType<PlayerController>().All(x => x.IsReady);
    }

    private IEnumerator CountDown()
    {
        if (timer)
            timer.text = timeRemaining.ToString();
        while (timeRemaining > 0){
            yield return new WaitForSeconds(1);
            if (timeRemaining > 1)
                AudioManager.instance.PlaySingle(countdown);
            else
                AudioManager.instance.PlaySingle(countdownFinal);
            --timeRemaining;
        }

        yield return new WaitForSeconds(1);
        GameManager.Instance.StartGame(LeadingLevels());
    }

    private void SpawnLevelPlatforms()
    {
        int count = levels.Count;
        int levelsRemaining = count;
        int levelIndex = 0;
        int platforms = (int)Mathf.Ceil(((float)count) / 2);
        float gap = areaWidth / (platforms + 1);
        float xValue = transform.position.x - areaWidth / 2;

        while (levelsRemaining > 0)
        {
            xValue += gap;
            Vector3 pos = new Vector3(xValue, 0, 0);
            Instantiate(platform, pos, Quaternion.identity);

            if (levelsRemaining > 1)
            {
                //instantiate the levels
                for (int y = 1; y > -2; y -= 2)
                {
                    Vector3 zonePos = pos;
                    zonePos += new Vector3(0, y * 1.25f, 1);
                    LevelZoneController zone = Instantiate(levelZone, zonePos, Quaternion.identity).GetComponent<LevelZoneController>();
                    zones.Add(zone);
                    zone.levelName = levels[levelIndex].levelName;
                    zone.GetComponent<MeshRenderer>().material = levels[levelIndex].previewMaterial;
                    ++levelIndex;
                    --levelsRemaining;
                }
            }
            else if (levelsRemaining == 1)
            {
                Vector3 zonePos = pos;
                zonePos += new Vector3(0, 1.25f, 0);
                LevelZoneController zone = Instantiate(levelZone, zonePos, Quaternion.identity).GetComponent<LevelZoneController>();
                zones.Add(zone);
                zone.levelName = levels[levelIndex].levelName;
                zone.GetComponent<MeshRenderer>().material = levels[levelIndex].previewMaterial;
                ++levelIndex;
                --levelsRemaining;
            }
        }
    }
}
