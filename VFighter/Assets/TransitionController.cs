using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TransitionController : MonoBehaviour {

    public VideoClip LevelLoadDisplacmentMap;
    public VideoClip LevelUnloadDisplacmentMap;
    public static TransitionController Instance;

    public GameObject TransitionRenderer;

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
        StartCoroutine(PlayAndWaitForVideoToEnd(OnFinish));
    }

    public void StartLoadLevelTransition(Action OnFinish)
    {
        GetComponent<VideoPlayer>().clip = LevelLoadDisplacmentMap;
        StartCoroutine(PlayAndWaitForVideoToEnd(OnFinish));
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
    }
}
