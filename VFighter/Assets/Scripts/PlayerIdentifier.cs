using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdentifier : MonoBehaviour {
    public List<Material> PlayerIdentifierMaterials;
    public int MaterialIndex;
    static int MaterialIndexCnt = 0;
    private void Start()
    {
        MaterialIndex = MaterialIndexCnt++;
    }
    private void Update()
    {
        GetComponent<Renderer>().material = PlayerIdentifierMaterials[MaterialIndex];
    }

}
