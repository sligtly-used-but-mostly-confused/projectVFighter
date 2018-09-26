using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConductivePlatformController : MonoBehaviour {

    private bool isCharged;

    public int chargeLength = 2;
    public Material unchargedMaterial;
    public Material chargedMaterial;

    public void charge(){
        StartCoroutine(ChargeRoutine());
    }

    IEnumerator ChargeRoutine()
    {
        Debug.Log("charging");
        isCharged = true;
        gameObject.GetComponent<Renderer>().material = chargedMaterial;
        yield return new WaitForSeconds(chargeLength);
        gameObject.GetComponent<Renderer>().material = unchargedMaterial;
        isCharged = false;
        Debug.Log("done charging");
    }
}


