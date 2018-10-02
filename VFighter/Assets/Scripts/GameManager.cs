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
        //PlayerManager.Instance.ResetPlayers();
        //ProgressionThroughGame = (float)PlayerManager.Instance.Players.Max(x => x.NumDeaths) / (float)PlayerManager.Instance.Players.First().NumLives;
        NetworkManager.singleton.ServerChangeScene(levelName);
        //SceneManager.LoadScene(levelName);
    }

    public void LoadNextStage()
    {
        /*
        var alive = PlayerManager.Instance.Players.Where(x => { return (x.NumLives - x.NumDeaths) > 0; });
        
        if(alive.Count() <= 1)
        {
            ControllerSelectManager.Instance.Init();
            SceneManager.LoadScene(LevelSelect);
            PlayerManager.Instance.ResetPlayers();
            return;
        }

        ProgressionThroughGame = (float)PlayerManager.Instance.Players.Max(x => x.NumDeaths) / (float) PlayerManager.Instance.Players.First().NumLives;
        SceneManager.LoadScene(_levelName);
        */
    }

}
