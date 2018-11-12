using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;

public class StartTutorialLobby : MonoBehaviour {

    private void Awake()
    {
        CustomNetworkManager.Instance.StartHost();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
