using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TronTail : NetworkBehaviour {

    public float SecondsForTailToSurvive;

    // Use this for initialization
    public override void OnStartServer()
    {
        base.OnStartServer();
        StartCoroutine(DestroyAfterWait());
    }

    private IEnumerator DestroyAfterWait()
    {
        yield return new WaitForSeconds(SecondsForTailToSurvive);
        NetworkServer.Destroy(gameObject);
    }
}
