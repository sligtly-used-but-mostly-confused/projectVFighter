using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelateImageEffect : MonoBehaviour {
    public Material effectMaterial;
    int number = 5;
	// Use this for initialization
    [ExecuteInEditMode]
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnRenderImage(RenderTexture src, RenderTexture  dst)
    {
        Graphics.Blit(src, dst, effectMaterial);
    }
}
