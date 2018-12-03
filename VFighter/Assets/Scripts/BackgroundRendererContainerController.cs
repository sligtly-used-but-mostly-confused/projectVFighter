using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundRendererContainerController : MonoBehaviour {
    public static BackgroundRendererContainerController Instance;
	// Use this for initialization
	void Start () {
        if(Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        if(GameRoundSettingsController.Instance.LoadBackgounds)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
