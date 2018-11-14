using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    private const string LevelSelect = "ControllerSelect";
    private const string EndScoreScreen = "EndScoreScreen";

    [SerializeField]
    private string _levelName;
    [SerializeField]
    private List<string> _roundLevelNames = new List<string>();
    public bool DebugScene = false;
    public bool CurrentlyChangingScenes = false;
    public float ProgressionThroughGame = 1;
    public bool CanChangeCharacters = true;
    public float TimeScale = 1;
    public int RoundNumber = 0;
    public int NumRounds = 0;

    public delegate void PlayerJoinCallback(PlayerController player);
    public PlayerJoinCallback OnPlayerJoin;

    void Awake () {
        if(_instance)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this);
	}

    public void StartGame(List<string> roundStages)
    {
        RoundNumber = 1;
        _roundLevelNames = roundStages;
        NumRounds = _roundLevelNames.Count();
        LoadNewLevel();
        CanChangeCharacters = false;
        GameObject.FindObjectsOfType<TutorialPromptController>().ToList().ForEach(x => x.gameObject.SetActive(false));
    }

    public void LoadEndScoreScreen()
    {
        CurrentlyChangingScenes = true;
        // NetworkManager.singleton.ServerChangeScene(EndScoreScreen);
        SceneManager.LoadScene(EndScoreScreen);
        CanChangeCharacters = false;
    }

    public void EndGame()
    {
        var players = FindObjectsOfType<PlayerController>().ToList();
        players.ForEach(x => x.ControlledPlayer.ResetForNewGame());
        CurrentlyChangingScenes = true;
        SceneManager.LoadScene(LevelSelect);
        CanChangeCharacters = true;
    }

    public void LoadNextStage()
    {
        var players = FindObjectsOfType<PlayerController>().ToList();
        var alive = players.Where(x => { return (x.ControlledPlayer.NumLives - x.ControlledPlayer.NumDeaths) > 0; }).ToList();
        StartNewRound();
    }

    private void StartNewRound()
    {
        var players = FindObjectsOfType<PlayerController>().ToList();
        players.Where(x => !x.IsDead).ToList().ForEach(x => x.ControlledPlayer.NumStageWins++);

        if (_roundLevelNames.Count > 0)
        {
            RoundNumber++;
            LoadNewLevel();
        }
        else
        {
            LoadEndScoreScreen();
        }
    }
    
    private void LoadNewLevel()
    {
        var players = FindObjectsOfType<PlayerController>().ToList();
        _levelName = _roundLevelNames[0];
        _roundLevelNames.RemoveAt(0);
        players.ForEach(x => x.ControlledPlayer.Reset());
        StartNewLevel();
    }

    private void StartNewLevel()
    {
        var players = FindObjectsOfType<PlayerController>().ToList();
        ProgressionThroughGame = players.Max(x => x.ControlledPlayer.NumDeaths) / (float)players[0].ControlledPlayer.NumLives;
        players.ForEach(x => x.IsDead = false);
        CurrentlyChangingScenes = true;
        SceneManager.LoadScene(_levelName);
    }

    public void DoneChangingScenes()
    {
        CurrentlyChangingScenes = false;
    }
    /*
    public void CheckHeartBeatThenCallback(Action callback)
    {
        StartCoroutine(CheckHeartBeatThenCallbackInternal(callback));
    }
    
    private IEnumerator CheckHeartBeatThenCallbackInternal(Action callback)
    {
        var players = FindObjectsOfType<PlayerController>().ToList();
        List<Tuple<PlayerController, int>> heartBeatIds = new List<Tuple<PlayerController, int>>();
        foreach(var player in players)
        {
            heartBeatIds.Add(new Tuple<PlayerController, int>(player, player.GetHeartBeat()));
        }

        float time = 0;

        while(!heartBeatIds.All(x => x.Item1.HeartBeats[x.Item2]))
        {
            time += Time.deltaTime;

            //if the heart beat hangs retry it
            if(time > 1)
            {
                yield return CheckHeartBeatThenCallbackInternal(callback);
                yield break;
            }

            yield return new WaitForEndOfFrame();
        }

        callback();
    }
    */
}
