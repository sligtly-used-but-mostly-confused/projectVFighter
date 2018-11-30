using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MonoBehaviour {
    public string SceneName;
	
    public void Pressed()
    {
        SceneManagementController.Instance.ChangeScenesAsycBehindTransition(SceneName);
    }
}
