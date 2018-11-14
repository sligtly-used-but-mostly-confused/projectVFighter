using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ForceZoneController : MonoBehaviour
{
    public Vector2 gravityDirection = new Vector2(1, 1);
    public float Force = 4f;

    //Audio
    public AudioSource sfxAudio;
    public AudioClip[] ForceSound;
    public AudioClip[] ForceSoundCave;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<GravityObjectRigidBody>())
        {
            var changeInGravDirection = gravityDirection.normalized - collision.GetComponent<GravityObjectRigidBody>().GravityDirection;
            var newGravDirection = collision.GetComponent<GravityObjectRigidBody>().GravityDirection + changeInGravDirection * Force * Time.deltaTime;
            var players = FindObjectsOfType<PlayerController>();
            if (players.Count() > 0)
            {
                players.First().ChangeGORBGravityDirection(collision.GetComponent<GravityObjectRigidBody>(), newGravDirection);
                ForceSfx();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var players = FindObjectsOfType<PlayerController>();
        if (players.Count() > 0)
        {
            players.First().ChangeGORBGravityDirection(collision.GetComponent<GravityObjectRigidBody>(), gravityDirection.normalized);
        }
    }

    public Vector2 GetgravityForce(){
        return gravityDirection;
    }

    public void ForceSfx()
    {
        //Generate a random number between 0 and the length of our array of clips passed in.
        int randomIndex;
        if (AudioManager.instance.isCaveLevel)
            randomIndex = Random.Range(0, ForceSoundCave.Length);
        else
            randomIndex = Random.Range(0, ForceSound.Length);

        //Choose a random pitch to play back our clip at between our high and low pitch ranges.
        float randomPitch = Random.Range(AudioManager.instance.lowPitchRange, AudioManager.instance.highPitchRange);

        //Set the pitch of the audio source to the randomly chosen pitch.
        sfxAudio.pitch = randomPitch;

        //Set the clip to the clip at our randomly chosen index.
        if (AudioManager.instance.isCaveLevel)
            sfxAudio.clip = ForceSoundCave[randomIndex];
        else
            sfxAudio.clip = ForceSound[randomIndex];
        sfxAudio.volume = 1.0f * AudioManager.SFXVol * AudioManager.MasterVol;
        //Play the clip.
        sfxAudio.Play();
    }
}
