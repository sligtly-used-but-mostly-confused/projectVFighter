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
    

    public GameObject TransitionRendererContainer;

    private Coroutine TransitionCoroutine;
    
    public void StartTransition(Action OnFinish)
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
