using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllableGravityObjectRigidBody : GravityObjectRigidBody {
    [SerializeField]
    private float _speedMultiplier;
    [SerializeField]
    private float _speedMultiplierStep;
    [SerializeField]
    private float _maxSpeedMultiplier;

    public PlayerController LastShotBy;

    //Audio
    public AudioSource sfxAudio;
    public AudioClip[] LaunchSound;
    public AudioClip[] LaunchSoundCave;

    public void Start()
    {
        LastShotBy = null;
    }

    public void StepMultiplier()
    {
        _speedMultiplier *= 1 + _speedMultiplierStep;
        _speedMultiplier = Mathf.Clamp(_speedMultiplier, 1, _maxSpeedMultiplier);
    }

    public override void UpdateVelocity(VelocityType id, Vector2 vel)
    {
        if (!_velocities.ContainsKey(id))
        {
            _velocities.Add(id, vel);
        }

        _velocities[id] = vel;
    }

    public override Vector2 GetMaxComponentVelocity(VelocityType type)
    {
        return base.GetMaxComponentVelocity(type) * _speedMultiplier;
    }

    
    public override void AddVelocity(VelocityType id, Vector2 velocityVector)
    {
        base.AddVelocity(id, velocityVector * _speedMultiplier);
    }

    public void LaunchSfx()
    {
        //Generate a random number between 0 and the length of our array of clips passed in.
        int randomIndex;
        if (AudioManager.Instance.isCaveLevel)
            randomIndex = Random.Range(0, LaunchSoundCave.Length);
        else
            randomIndex = Random.Range(0, LaunchSound.Length);

        //Choose a random pitch to play back our clip at between our high and low pitch ranges.
        float randomPitch = Random.Range(AudioManager.Instance.lowPitchRange, AudioManager.Instance.highPitchRange);

        //Set the pitch of the audio source to the randomly chosen pitch.
        sfxAudio.pitch = randomPitch;

        //Set the clip to the clip at our randomly chosen index.
        if (AudioManager.Instance.isCaveLevel)
            sfxAudio.clip = LaunchSoundCave[randomIndex];
        else
            sfxAudio.clip = LaunchSound[randomIndex];
        sfxAudio.volume = 1.0f * AudioManager.SFXVol * AudioManager.MasterVol;
        //Play the clip.
        sfxAudio.Play();
    }
}
