using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class StartTutorialLobby : MonoBehaviour {
    public SceneAsset Tutorial;

    private void Awake()
    {
        CustomNetworkManager.Instance.StartHost();
        //SceneManager.LoadScene(Tutorial.name);

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
