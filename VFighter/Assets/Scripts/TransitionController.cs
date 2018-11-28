using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class TransitionController : MonoBehaviour {

    public VideoClip LevelUnloadDisplacmentMap;
    public VideoPlayer Player;
    public static TransitionController Instance;

    public GameObject TransitionRendererContainer;

    private Coroutine TransitionCoroutine;
    
    private void Awake()
    {
        if(Instance)
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
        StartUnloadLevelTransition(() =>
        {
            StartCoroutine(WaitForSceneToLoad(asyncLoad, oldSceneName, callback));
        });
    }

    private IEnumerator WaitForSceneToLoad(AsyncOperation asyncLoad, string oldSceneName, Action callback)
    {
        Debug.Log("asdasd");
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
        

        //Debug.Log($"unloading {oldSceneName}");
        //SceneManager.UnloadSceneAsync(oldSceneName);

        
        //var objs = FindObjectsOfType<ObjectsToBeLoadedAfterTransitionController>().ToList();
        //objs.ForEach(x => x.LoadChildren());
        
    }


    public void StartUnloadLevelTransition(Action OnFinish)
    {
        Player.clip = LevelUnloadDisplacmentMap;
        TransitionCoroutine = StartCoroutine(PlayAndWaitForVideoToEnd(OnFinish));
    }

    public bool IsCurrentlyTransitioning()
    {
        return TransitionCoroutine != null;
    }

    IEnumerator PlayAndWaitForVideoToEnd(Action OnFinish)
    {
        TransitionRendererContainer.SetActive(true);
        Player.Play();

        while (Player.isPlaying)
        {
            yield return new WaitForEndOfFrame();
        }

        TransitionRendererContainer.SetActive(false);
        OnFinish();
        TransitionCoroutine = null;
    }
}
