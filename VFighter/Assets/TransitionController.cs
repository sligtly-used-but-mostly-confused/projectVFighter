using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TransitionController : MonoBehaviour {

    public VideoClip LevelUnloadDisplacmentMap;
    public static TransitionController Instance;

    public GameObject TransitionRenderer;
    private Coroutine TransitionCoroutine;
    private void Awake()
    {
        if(Instance)
        {
            Destroy(Instance.gameObject);
        }

        Instance = this;
    }

    public void StartUnloadLevelTransition(Action OnFinish)
    {
        GetComponent<VideoPlayer>().clip = LevelUnloadDisplacmentMap;
        TransitionCoroutine = StartCoroutine(PlayAndWaitForVideoToEnd(OnFinish));
    }

    public bool IsCurrentlyTransitioning()
    {
        return TransitionCoroutine != null;
    }

    IEnumerator PlayAndWaitForVideoToEnd(Action OnFinish)
    {
        TransitionRenderer.SetActive(true);
        var player = GetComponent<VideoPlayer>();
        player.Play();

        while (player.isPlaying)
        {
            yield return new WaitForEndOfFrame();
        }
        
        TransitionRenderer.SetActive(false);
        OnFinish();
        TransitionCoroutine = null;
    }
}
