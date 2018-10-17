using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSize : MonoBehaviour {

    [SerializeField]
    private Camera cm;
    [SerializeField]
    private GameObject bg;
    public float camSize;
    public float xScale = 0.20f;
    public float yScale = 0.20f;
    void Start()
    {
        cm = GetComponent<Camera>();
    }
	// Update is called once per frame
	void Update () {

        bg.transform.localScale = new Vector3(cm.orthographicSize * xScale, cm.orthographicSize * yScale, 1);
	}
}
