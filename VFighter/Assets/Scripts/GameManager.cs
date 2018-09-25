using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameManager : MonoBehaviour {
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public List<string> StageSceneNames = new List<string>();

    public int StageNum = 0;
    public int MaxStageNum { get { return StageSceneNames.Count; } }

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
        for(int i = 0; i < numStages; i++)
        {
            StageSceneNames.Add(levelName);
        }

        SceneManager.LoadScene(levelName);
    }

    public void LoadNextStage()
    {
        StageNum++;
        if(StageNum > StageSceneNames.Count)
        {
            ControllerSelectManager.Instance.Init();
            SceneManager.LoadScene(LevelSelect);
            return;
        }
        SceneManager.LoadScene(StageSceneNames[StageNum]);
    }

}
