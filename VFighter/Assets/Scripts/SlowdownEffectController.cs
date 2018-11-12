using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SlowdownEffectController : MonoBehaviour {

    public GravityObjectRigidBody AttachedGORB;

    static bool IsSlowDownCurrentlyRunning = false;

    [SerializeField]
    private float _slowdownDuration = .5f;
    [SerializeField]
    private float _slowdownTimeScale = .1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.name + " " + IsSlowDownCurrentlyRunning);
        if(AttachedGORB.isServer && collision.GetComponent<PlayerController>() && !IsSlowDownCurrentlyRunning)
        {
            StartCoroutine(SlowTimeEffect());
        }
    }

    public IEnumerator SlowTimeEffect()
    {
        IsSlowDownCurrentlyRunning = true;
        var prevTimeScale = GameManager.Instance.TimeScale;
        GameManager.Instance.TimeScale = _slowdownTimeScale;
        yield return new WaitForSeconds(_slowdownDuration);
        GameManager.Instance.TimeScale = prevTimeScale;
        IsSlowDownCurrentlyRunning = false;
    }

    public void OnDestroy()
    {
        //make sure the effect doesent get frozen
        IsSlowDownCurrentlyRunning = false;
        GameManager.Instance.TimeScale = 1;
    }
}
