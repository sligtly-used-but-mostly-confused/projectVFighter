using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TransitionController : MonoBehaviour {

	public void StartTransition(Action OnFinish)
    {
        StartCoroutine(PlayAndWaitForVideoToEnd(OnFinish));
    }

    IEnumerator PlayAndWaitForVideoToEnd(Action OnFinish)
    {
        GetComponent<Renderer>().enabled = true;
        var player = GetComponent<VideoPlayer>();
        //player.clip.length
        player.Play();
        while (player.isPlaying)
        {
            yield return new WaitForEndOfFrame();
        }
        GetComponent<Renderer>().enabled = false;
        OnFinish();
    }
}
