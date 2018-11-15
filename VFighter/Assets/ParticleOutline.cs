using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleOutline : MonoBehaviour {
    [SerializeField]
    public SkinnedMeshRenderer smr;
    [SerializeField]
    public GameObject pb;

    private ParticleSystem ps;
    private GameObject gb;
    // Use this for initialization
    void Start () {
        smr = GetComponentInChildren<SkinnedMeshRenderer>();
        gb = Instantiate(pb);
        DontDestroyOnLoad(gb);
        gb.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }
	
	// Update is called once per frame
	void Update () {
        smr = GetComponentInChildren<SkinnedMeshRenderer>();
        gb.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        ps = gb.GetComponentInChildren<ParticleSystem>();
        var main = ps.shape;
        main.skinnedMeshRenderer = smr;
        ParticleSystem.ShapeModule _editableShape = ps.shape;
        _editableShape.position = new Vector3(0.0f, 0f, 0.0f);
        ps.Play();
    }
}
