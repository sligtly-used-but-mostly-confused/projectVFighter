using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TeleportZoneController : MonoBehaviour {

    public TeleportZoneController TeleportTo;
    public List<GravityObjectRigidBody> ObjectsWaitingToExitTeleporter = new List<GravityObjectRigidBody>();

    //Audio
    public AudioSource sfxAudio;
    public AudioClip[] TeleportSound;
    public AudioClip[] TeleportSoundCave;

    [SerializeField]
    public Vector2 ForceKickDirection = Vector2.zero;
    [SerializeField]
    public float ForceKickScale = 100;

    [SerializeField]
    private float _objectExitFromTeleporterTimeout = .1f;

    public UnityEvent OnTeleport; 

    private void OnTriggerStay2D(Collider2D collision)
    {
        Rigidbody2D rb = collision.attachedRigidbody;
        var gorb = collision.GetComponent<GravityObjectRigidBody>();
        if (gorb && !ObjectsWaitingToExitTeleporter.Contains(gorb))
        {
            Debug.Log("teleporting");
            TeleportTo.ObjectsWaitingToExitTeleporter.Add(gorb);

            //get the players offset from the teleporter so we can match it on the other side
            //also do a projection with the scale of the teleporter so that the player is garunteed to spawn 
            //within the teleporter when they come out
            var deltaPosition = collision.transform.position - transform.position;
            var teleporterScale = TeleportTo.transform.rotation * TeleportTo.transform.lossyScale;
            teleporterScale = new Vector3(Mathf.Abs(teleporterScale.x), Mathf.Abs(teleporterScale.y), Mathf.Abs(teleporterScale.z));
            var playerOffsetFromTeleporter = Vector3.Project(deltaPosition, teleporterScale);

            //gorb.AddVelocity(VelocityType.OtherPhysics, TeleportTo.ForceKickDirection * TeleportTo.ForceKickScale);
            collision.transform.position = TeleportTo.transform.position + playerOffsetFromTeleporter;

            TeleportTo.StartCoroutine(TeleportTo.ObjectExitTeleporterTimeout(gorb));
            OnTeleport.Invoke();
            TeleportSfx();
        }
    }

    public IEnumerator ObjectExitTeleporterTimeout(GravityObjectRigidBody gorb)
    {
        yield return new WaitForSeconds(_objectExitFromTeleporterTimeout);

        //unity derped and didnt trigger the on trigger exit
        if (gorb && gorb.isActiveAndEnabled && ObjectsWaitingToExitTeleporter.Contains(gorb))
        {
            ObjectsWaitingToExitTeleporter.Remove(gorb);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        var gorb = collision.GetComponent<GravityObjectRigidBody>();
        if (gorb && ObjectsWaitingToExitTeleporter.Contains(gorb))
        {
            ObjectsWaitingToExitTeleporter.Remove(gorb);
        }
    }

    public void TeleportSfx()
    {
        //Generate a random number between 0 and the length of our array of clips passed in.
        int randomIndex;
        if (AudioManager.Instance.isCaveLevel)
            randomIndex = Random.Range(0, TeleportSoundCave.Length);
        else
            randomIndex = Random.Range(0, TeleportSound.Length);

        //Choose a random pitch to play back our clip at between our high and low pitch ranges.
        float randomPitch = Random.Range(AudioManager.Instance.lowPitchRange, AudioManager.Instance.highPitchRange);

        //Set the pitch of the audio source to the randomly chosen pitch.
        sfxAudio.pitch = randomPitch;

        //Set the clip to the clip at our randomly chosen index.
        if (AudioManager.Instance.isCaveLevel)
            sfxAudio.clip = TeleportSoundCave[randomIndex];
        else
            sfxAudio.clip = TeleportSound[randomIndex];
        sfxAudio.volume = 1.0f * AudioManager.SFXVol * AudioManager.MasterVol;
        //Play the clip.
        sfxAudio.Play();
    }
}
