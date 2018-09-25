using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class AudioManager : MonoBehaviour
{
    public AudioSource mainAudio;                   //Drag a reference to the audio source which will play the sound effects.
    public static AudioManager instance = null;
    bool isPlaying = false;
    void Awake()
    {
        //Check if there is already an instance of SoundManager
        if (instance == null)
            //if not, set it to this.
            instance = this;
        //If instance already exists:
        else if (instance != this)
            //Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
            Destroy(gameObject);

        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad(gameObject);
        mainAudio.loop = true;
    }
    // Use this for initialization
    void Start()
    {
        mainAudio.volume = 0.4f;
        mainAudio.Play();
       
    }



    

}
