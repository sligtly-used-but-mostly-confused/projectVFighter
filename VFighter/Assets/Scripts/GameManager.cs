using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameManager : MonoBehaviour {
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public List<PlayerController> players;

	void Awake () {
        //if (_instance == null)
       // {
        //    DontDestroyOnLoad(gameObject);
            _instance = this;
        //}
        //else
        //{
           // Destroy(gameObject);
        //}
	}

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        var alive = players.Where(x => !x.IsDead);
        if (alive.Count() <= 1)
        {
            Debug.Log(alive.First()?.name + " won");
            ResetLevel();
        }
    }

    private void Init()
    {
        players = new List<PlayerController>(GameObject.FindObjectsOfType<PlayerController>());
    }

    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
       // Init();
    }
}
