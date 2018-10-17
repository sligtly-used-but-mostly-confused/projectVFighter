using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleWithCamera : MonoBehaviour {

    public Transform tran;
    public Camera cam;
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        //tran.localScale.x = 18 * (double)cam.orthographicSize;
        //tran.localScale.y = 10.125 * (double)cam.orthographicSize;
	}
}
