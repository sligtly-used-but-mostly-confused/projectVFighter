using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static float MusicVol;
    public static float SFXVol;
    public static float MasterVol;

    [SerializeField, Tooltip("If when the AM loads and another AM is present this AM will be kept and the other will be destroyed")]
    private bool _defaultToThisAudioManager = true;

    public AudioSource mainAudio;                   //Used for music
    public AudioSource mainAudio2;
    public AudioSource sfxAudio;                    //Used for sound FX
    public static AudioManager Instance = null;
    bool isPlaying = false;
    public bool isCaveLevel = false;                // set to true when reverberated sound effects should be used
    public float lowPitchRange = .95f;              //The lowest a sound effect will be randomly pitched.
    public float highPitchRange = 1.05f;            //The highest a sound effect will be randomly pitched.

    //Music
    public AudioClip defaultRoundInit;
    public AudioClip defaultRoundLoop;
    public AudioClip finalRoundInit;
    public AudioClip finalRoundLoop;
    public bool hasInit;
    public bool hasFinRndVer;
    private double nextEventTime;

    //Global Collision Clips
    public AudioClip[] Coll;
    public AudioClip[] CollCave;
    public AudioClip transition;

    void Awake()
    {
        //Check if there is already an instance of SoundManager
        if (Instance == null)
            //if not, set it to this.
            Instance = this;
        //If instance already exists:
        else if (Instance != this)
        {
            if(_defaultToThisAudioManager)
            {
                //Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
                Destroy(Instance.gameObject);
                Instance = this;
            }
            else
            {

                Destroy(gameObject);
                return;
            }
        }

        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad(gameObject);
        mainAudio.loop = !hasInit;
    }
    // Use this for initialization
    void Start()
    {
        if (MasterVol == 0.0f)
        {
            MusicVol = 1.0f;
            SFXVol = 1.0f;
            MasterVol = 1.0f;
        }
        
        mainAudio.volume = 0.4f * MusicVol * MasterVol;
        if(hasFinRndVer && (GameManager.Instance.NumRounds == GameManager.Instance.RoundNumber))
        {
            //play final round version
            if (hasInit)
                mainAudio.clip = finalRoundInit;
            else
                mainAudio.clip = finalRoundLoop;
        }
        else
        {
            if (hasInit)
                mainAudio.clip = defaultRoundInit;
            else
                mainAudio.clip = defaultRoundLoop;
        }
        playSong();
        
    }

    void Update()
    {
        mainAudio.volume = 0.4f * MusicVol * MasterVol;
        mainAudio2.volume = mainAudio.volume;
        sfxAudio.volume = 1.0f * SFXVol * MasterVol;

        double time = AudioSettings.dspTime;
        if (!mainAudio2.isPlaying && hasInit && time + 1.0f > nextEventTime)
        {
            mainAudio2.PlayScheduled(nextEventTime);
        }
        }

    // Sound FX functions
    //Used to play single sound clips.
    public void PlaySingle(AudioClip clip)
    {
        //Set the clip of our efxSource audio source to the clip passed in as a parameter.
        sfxAudio.clip = clip;
        sfxAudio.pitch = 1;
        //Play the clip.
        sfxAudio.Play();
    }

    public void PlaySingle(AudioClip clip, float pitch)
    {
        //Set the clip of our efxSource audio source to the clip passed in as a parameter.
        sfxAudio.clip = clip;
        sfxAudio.pitch = pitch;
        //Play the clip.
        sfxAudio.Play();
    }


    //RandomizeSfx chooses randomly between various audio clips using the global audio channel
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

    public void SetMasterVol(float v)
    {
        MasterVol = v;
    }

    public void SetMusicVol(float v)
    {
        MusicVol = v;
    }

    public void SetSFXVol(float v)
    {
        SFXVol = v;
    }

    // same as above except you can specify an audio source (used for collisions)
    public void RandomizeSfx(AudioClip[] def, AudioClip[] cave, AudioSource src)
    {
        //Generate a random number between 0 and the length of our array of clips passed in.
        int randomIndex;
        if (AudioManager.Instance.isCaveLevel)
            randomIndex = Random.Range(0, cave.Length);
        else
            randomIndex = Random.Range(0, def.Length);

        //Choose a random pitch to play back our clip at between our high and low pitch ranges.
        float randomPitch = Random.Range(AudioManager.Instance.lowPitchRange, AudioManager.Instance.highPitchRange);

        //Set the pitch of the audio source to the randomly chosen pitch.
        src.pitch = randomPitch;

        //Set the clip to the clip at our randomly chosen index.
        if (AudioManager.Instance.isCaveLevel)
            src.clip = cave[randomIndex];
        else
            src.clip = def[randomIndex];
        src.volume = 1.0f * AudioManager.SFXVol * AudioManager.MasterVol;
        //Play the clip.
        src.Play();
    }

    public void playSong()
    {
        if (hasInit)
        {
            if (hasFinRndVer && (GameManager.Instance.NumRounds == GameManager.Instance.RoundNumber))
                mainAudio2.clip = finalRoundLoop;
            else
                mainAudio2.clip = defaultRoundLoop;
            mainAudio2.pitch = mainAudio.pitch;
            mainAudio2.loop = true;
        }
        mainAudio.Play();
        double len;
        if (mainAudio.pitch != 1.0f)
            len = mainAudio.clip.length / mainAudio.pitch + 0.25;
        else
            len = mainAudio.clip.length;
        nextEventTime = AudioSettings.dspTime + len;
    }
}

