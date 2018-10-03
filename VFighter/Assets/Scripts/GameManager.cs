using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameManager : NetworkBehaviour {
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public float ProgressionThroughGame = 1;

    [SerializeField]
    private string _levelName;
    private const string LevelSelect = "ControllerSelect";
    public bool CurrentlyChangingScenes = false;
    void Awake () {
        if(_instance)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this);
	}

    [SyncVar]
    public float TimeScale = 1;

    public void StartGame(string levelName, int numStages)
    {
        _levelName = levelName;
        //PlayerManager.Instance.ResetPlayers();
        //ProgressionThroughGame = (float)PlayerManager.Instance.Players.Max(x => x.NumDeaths) / (float)PlayerManager.Instance.Players.First().NumLives;
        NetworkManager.singleton.ServerChangeScene(levelName);
        //SceneManager.LoadScene(levelName);
    }

    public void LoadNextStage()
    {
        var players = FindObjectsOfType<PlayerController>().ToList();
        var alive = players.Where(x => { return (x.ControlledPlayer.NumLives - x.ControlledPlayer.NumDeaths) > 0; }).ToList();
        
        if(alive.Count() <= 1)
        {
            ControllerSelectManager.Instance.Init();
            //SceneManager.LoadScene(LevelSelect);
            players.ForEach(x => x.ControlledPlayer.Reset());
            Debug.Log("change scene");
            CurrentlyChangingScenes = true;
            NetworkManager.singleton.ServerChangeScene(LevelSelect);
            //CurrentlyChangingScenes = false;
            return;
        }
        

        //ProgressionThroughGame = (float)PlayerManager.Instance.Players.Max(x => x.NumDeaths) / (float) PlayerManager.Instance.Players.First().NumLives;
        ProgressionThroughGame = 1;
        //SceneManager.LoadScene(_levelName);
        players.ForEach(x => x.IsDead = false);
        NetworkManager.singleton.ServerChangeScene(_levelName);
    }

    public void DoneChangingScenes()
    {
        CurrentlyChangingScenes = false;
    }
    
}
