using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RocketProjectileController : GravityGunProjectileController
{
    //Audio
    public AudioSource sfxAudio;
    public AudioClip[] BlastSound;
    public AudioClip[] BlastSoundCave;

    public GameObject RocketBlastPrefab;
    public override void OnHitGORB(GravityObjectRigidBody GORB)
    {
        //BlastSfx();
    }

    protected override void ReturnToPool()
    {
        var blast = Instantiate(RocketBlastPrefab);
        blast.transform.position = this.transform.position + Vector3.back * 2;
        base.ReturnToPool();
    }
    public void BlastSfx()
    {
        //Generate a random number between 0 and the length of our array of clips passed in.
        int randomIndex;
        if (AudioManager.instance.isCaveLevel)
            randomIndex = Random.Range(0, BlastSoundCave.Length);
        else
            randomIndex = Random.Range(0, BlastSound.Length);

        //Choose a random pitch to play back our clip at between our high and low pitch ranges.
        float randomPitch = Random.Range(AudioManager.instance.lowPitchRange, AudioManager.instance.highPitchRange);

        //Set the pitch of the audio source to the randomly chosen pitch.
        sfxAudio.pitch = randomPitch;

        //Set the clip to the clip at our randomly chosen index.
        if (AudioManager.instance.isCaveLevel)
            sfxAudio.clip = BlastSoundCave[randomIndex];
        else
            sfxAudio.clip = BlastSound[randomIndex];
        sfxAudio.volume = 1.0f * AudioManager.SFXVol * AudioManager.MasterVol;
        //Play the clip.
        sfxAudio.Play();
    }
}
