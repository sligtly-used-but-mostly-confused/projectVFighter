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
        NetworkManager.singleton.ServerChangeScene(levelName);
    }

    public void LoadNextStage()
    {
        var players = FindObjectsOfType<PlayerController>().ToList();
        var alive = players.Where(x => { return (x.ControlledPlayer.NumLives - x.ControlledPlayer.NumDeaths) > 0; }).ToList();
        
        if(alive.Count() <= 1)
        {
            ControllerSelectManager.Instance.Init();
            players.ForEach(x => x.ControlledPlayer.Reset());
            CurrentlyChangingScenes = true;
            NetworkManager.singleton.ServerChangeScene(LevelSelect);
            return;
        }

        ProgressionThroughGame = 1;
        players.ForEach(x => x.IsDead = false);
        NetworkManager.singleton.ServerChangeScene(_levelName);
    }

    public void DoneChangingScenes()
    {
        CurrentlyChangingScenes = false;
    }
    
}
