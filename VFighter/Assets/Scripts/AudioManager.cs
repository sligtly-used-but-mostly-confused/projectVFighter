using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class AudioManager : MonoBehaviour
{
    public AudioSource mainAudio;                   //Used for music
    public AudioSource sfxAudio;                    //Used for sound FX
    public static AudioManager instance = null;
    bool isPlaying = false;
    public bool isCaveLevel = false;                // set to true when reverberated sound effects should be used
    public float lowPitchRange = .95f;              //The lowest a sound effect will be randomly pitched.
    public float highPitchRange = 1.05f;            //The highest a sound effect will be randomly pitched.

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

    // Sound FX functions
    //Used to play single sound clips.
    public void PlaySingle(AudioClip clip)
    {
        //Set the clip of our efxSource audio source to the clip passed in as a parameter.
        sfxAudio.clip = clip;

        //Play the clip.
        sfxAudio.Play();
    }


    //RandomizeSfx chooses randomly between various audio clips
    public void RandomizeSfx(AudioClip[] clips, AudioClip[] caveClips)
    {
        //Generate a random number between 0 and the length of our array of clips passed in.
        int randomIndex;
        if (isCaveLevel)
            randomIndex = Random.Range(0, caveClips.Length);
        else
            randomIndex = Random.Range(0, clips.Length);

        //Choose a random pitch to play back our clip at between our high and low pitch ranges.
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        //Set the pitch of the audio source to the randomly chosen pitch.
        sfxAudio.pitch = randomPitch;

        //Set the clip to the clip at our randomly chosen index.
        if (isCaveLevel)
            sfxAudio.clip = caveClips[randomIndex];
        else
            sfxAudio.clip = clips[randomIndex];

        //Play the clip.
        sfxAudio.Play();
    }

    public void ChangeMusic(AudioClip clip)
    {
        //Set the clip of our efxSource audio source to the clip passed in as a parameter.
        mainAudio.clip = clip;

        //Play the clip.
        mainAudio.Play();
    }

}
