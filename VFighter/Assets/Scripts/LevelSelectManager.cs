using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;


public class LevelSelectManager : NetworkBehaviour
{
    public static LevelSelectManager Instance;

    public List<string> levels;
    public GameObject platform;
    public GameObject levelZone;
    public float areaWidth;
    public Text timer;
    public int selectTime;
    public bool IsTimerStarted { get { return _timerCoroutine != null; } }

    private List<LevelZoneController> zones = new List<LevelZoneController>();
    private Coroutine _timerCoroutine;

    [SerializeField, SyncVar]
    private int timeRemaining;

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
    }

    private void Update()
    {
        timer.text = timeRemaining.ToString();
    }

    private string LeadingLevel(){
        LevelZoneController leader = zones[0];
        int mostVotes = 0;
        foreach(LevelZoneController zone in zones){
            if (zone.playersInside > mostVotes){
                leader = zone;
                mostVotes = zone.playersInside;
            }
        }
        return leader.levelName;
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

    private IEnumerator CountDown()
    {
        timer.text = timeRemaining.ToString();
        while (timeRemaining > 0){
            yield return new WaitForSeconds(1);
            --timeRemaining;
        }

        yield return new WaitForSeconds(1);
        GameManager.Instance.StartGame(LeadingLevel(), 10);
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
                    zonePos += new Vector3(0, y * 1.25f, 0);
                    LevelZoneController zone = Instantiate(levelZone, zonePos, Quaternion.identity).GetComponent<LevelZoneController>();
                    zones.Add(zone);
                    zone.levelName = levels[levelIndex];
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
                zone.levelName = levels[levelIndex];
                ++levelIndex;
                --levelsRemaining;
            }
        }
    }
}
