using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagementController : MonoBehaviour
{
    public static SceneManagementController Instance;
    [SerializeField]
    private TransitionController _tController;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(Instance.gameObject);
        }

        Instance = this;
    }

    public void ChangeScenesAsycBehindTransition(string newScene)
    {
        ChangeScenesAsycBehindTransition(newScene, () => { });
    }

    public void ChangeScenesAsycBehindTransition(string newScene, Action callback)
    {
        string oldSceneName = SceneManager.GetActiveScene().name;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(newScene);
        asyncLoad.allowSceneActivation = false;
        _tController.StartTransition(() =>
        {
            StartCoroutine(WaitForSceneToLoad(asyncLoad, oldSceneName, callback));
        });
    }

    public bool IsCurrentlyTransitioning()
    {
        return _tController.IsCurrentlyTransitioning();
    }

    private IEnumerator WaitForSceneToLoad(AsyncOperation asyncLoad, string oldSceneName, Action callback)
    {
        while (!asyncLoad.isDone)
        {
            yield return null;
            if (asyncLoad.progress >= 0.9f)
            {
                Debug.Log("asdasd");
                callback();
                asyncLoad.allowSceneActivation = true;
            }
        }
    }
}
