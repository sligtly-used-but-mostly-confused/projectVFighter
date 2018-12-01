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
    public Image timerClockFace;

    public int selectTime;
    public bool IsTimerStarted { get { return _timerCoroutine != null; } }
    
    public AudioClip countdown;
    public AudioClip countdownFinal;

    private List<LevelZoneController> zones = new List<LevelZoneController>();
    private Coroutine _timerCoroutine;

    [SerializeField]
    private GameObject _playerReadyIndicatorPrefab;
    
    [SerializeField]
    private bool _isWaitingForReady = true;
    [SerializeField]
    private float timeRemaining;
    private int _minPlayers { get { return GameRoundSettingsController.Instance.MinPlayers; } }
    private int _numRounds { get { return GameRoundSettingsController.Instance.NumRounds; } }
    private int _numLivesPerRound { get { return GameRoundSettingsController.Instance.NumLivesPerRound; } }

    private void Awake()
    {
        if(Instance)
        {
            Destroy(gameObject);
        }

        Instance = this;
        SceneManager.activeSceneChanged += OnLevelFinishedLoading;
    }

    void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene previousScene, Scene newScene)
    {
        if (LevelManager.Instance.ShowTutorialPrompt == true)
        {
            SpawnLevelPlatforms();
            timeRemaining = selectTime;
        }

        if(newScene.name != "ControllerSelect")
        {
            return;
        }

        SpawnLevelPlatforms();
        timeRemaining = selectTime;

        //the first time we load the game this list will be empty, but after that it will have players
        FindObjectsOfType<PlayerController>().ToList().ForEach(x =>
        {
            var indicator = Instantiate(_playerReadyIndicatorPrefab);
            indicator.GetComponent<PlayerReadyIndicatorController>().AttachedPlayer = x;

            x.IsReady = false;
            x.GetComponent<GravityObjectRigidBody>().CanMove = true;
        });

        RefreshRoundSettings();
    }

    public void RefreshRoundSettings()
    {
        var players = FindObjectsOfType<PlayerController>().ToList();
        players.ForEach(x => x.ControlledPlayer.NumLives = _numLivesPerRound);
    }

    private void Update()
    {
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

        copy.Reverse();

        var LevelsThatGotVotes = copy.Where(x => x.playersInside > 0);
        var LevelsThatGotNoVotes = copy.Where(x => x.playersInside == 0).ToList();

        var finalLevelSelection = new List<string>();
        finalLevelSelection.AddRange(LevelsThatGotVotes.Select(x => x.levelName).Take(_numRounds));
        
        while(finalLevelSelection.Count() < _numRounds)
        {
            int randIndex = Mathf.RoundToInt(Random.Range(0,LevelsThatGotNoVotes.Count()));
            var randLevel = LevelsThatGotNoVotes[randIndex];
            finalLevelSelection.Add(randLevel.levelName);
        }

        return finalLevelSelection;
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
        float startingTime = timeRemaining;

        if (timer)
            timer.text = "" + timeRemaining;

        int valToPass = (int) timeRemaining;

        while (true)
        {
            yield return new WaitForEndOfFrame();
            if (timeRemaining < valToPass)
            {
                if(valToPass == 1)
                {
                    AudioManager.Instance.PlaySingle(countdownFinal);
                }
                if(valToPass < 0)
                {
                    timer.text = "0";
                    timeRemaining = 0;
                    GameManager.Instance.StartGame(LeadingLevels());
                }
                else
                {
                    AudioManager.Instance.PlaySingle(countdown);
                }
                
                valToPass--;
                timer.text = "" + Mathf.RoundToInt(timeRemaining);
            }
            
            timeRemaining -= Time.deltaTime * GameManager.Instance.TimeScale;
            timerClockFace.rectTransform.Rotate(new Vector3(0, 0, startingTime / Mathf.Clamp( timeRemaining, .01f, startingTime)), Space.World);
        }
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
