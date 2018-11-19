using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.PostProcessing;

public class SlowdownEffectController : MonoBehaviour {

    public GravityObjectRigidBody AttachedGORB;

    static bool IsSlowDownCurrentlyRunning = false;

    [SerializeField]
    private float _slowdownDuration = .5f;
    [SerializeField]
    private float _slowdownCooldownDuration = 1f;
    [SerializeField]
    private float _slowdownTimeScale = .1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<PlayerController>() && 
            !IsSlowDownCurrentlyRunning && 
            !collision.GetComponent<PlayerCooldownController>().IsCoolingDown(CooldownType.Dash))
        {
            StartCoroutine(SlowTimeEffect());
        }
    }

    public IEnumerator SlowTimeEffect()
    {
        IsSlowDownCurrentlyRunning = true;
        var prevTimeScale = GameManager.Instance.TimeScale;
        GameManager.Instance.TimeScale = _slowdownTimeScale;
        PostProcessVolume vol = FindObjectOfType<PostProcessVolume>();
        Bloom bloom = null;
        ChromaticAberration CA = null;
        vol.profile.TryGetSettings(out bloom);
        vol.profile.TryGetSettings(out CA);
        bloom.intensity.value = 15;
        CA.intensity.value = 0.4f;
        yield return new WaitForSeconds(_slowdownDuration);
        bloom.intensity.value = 7.5f;
        CA.intensity.value = 0.1f;
        GameManager.Instance.TimeScale = prevTimeScale;
        yield return new WaitForSeconds(_slowdownCooldownDuration);
        IsSlowDownCurrentlyRunning = false;
    }

    public void OnDestroy()
    {
        //make sure the effect doesent get frozen
        IsSlowDownCurrentlyRunning = false;
        GameManager.Instance.TimeScale = 1;
    }
}
