using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TronTail : MonoBehaviour
{

    public float SecondsForTailToSurvive;

    // Use this for initialization
    public void Start()
    {
        StartCoroutine(DestroyAfterWait());
    }

    private IEnumerator DestroyAfterWait()
    {
        yield return new WaitForSeconds(SecondsForTailToSurvive);
        NetworkServer.Destroy(gameObject);
    }
}
