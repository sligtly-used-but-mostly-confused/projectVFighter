using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    private const string LevelSelect = "ControllerSelect";

    [SerializeField]
    private string _levelName;
    
    public bool CurrentlyChangingScenes = false;
    public float ProgressionThroughGame = 1;

    [SyncVar]
    public float TimeScale = 1;

    void Awake () {
        if(_instance)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this);
	}

    public void StartGame(string levelName, int numStages)
    {
        _levelName = levelName;
        LoadNextStage();
    }

    public void LoadNextStage()
    {
        var players = FindObjectsOfType<PlayerController>().ToList();
        var alive = players.Where(x => { return (x.ControlledPlayer.NumLives - x.ControlledPlayer.NumDeaths) > 0; }).ToList();
        
        if(alive.Count() <= 1)
        {
            CheckHeartBeatThenCallback(() =>
            {
                ControllerSelectManager.Instance.Init();
                players.ForEach(x => x.ControlledPlayer.Reset());
                CurrentlyChangingScenes = true;
                NetworkManager.singleton.ServerChangeScene(LevelSelect);
            });
        }
        else
        {
            CheckHeartBeatThenCallback(StartNewLevel);
        }
    }

    private void StartNewLevel()
    {
        var players = FindObjectsOfType<PlayerController>().ToList();
        ProgressionThroughGame = players.Max(x => x.ControlledPlayer.NumDeaths) / (float)players[0].ControlledPlayer.NumLives;
        players.ForEach(x => x.IsDead = false);
        CurrentlyChangingScenes = true;
        NetworkManager.singleton.ServerChangeScene(_levelName);
    }

    public void DoneChangingScenes()
    {
        CurrentlyChangingScenes = false;
    }

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
}
