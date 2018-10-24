using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class platformHoverEffect : MonoBehaviour {
    [SerializeField]
    private Transform currentPos;
    [SerializeField]
    private ParticleSystem ps;
    [SerializeField]
    private float initYPos;
    // Use this for initialization
    void Start () {
        currentPos = GetComponentInParent<Transform>();
        ps = GetComponent<ParticleSystem>();
        initYPos = currentPos.position.y;
        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.SingleSidedEdge;
        shape.radius = 0.08f;
    }
	
	// Update is called once per frame
	void Update () {
        var main = ps.main;
        main.startSpeed = 0.05f;
        var shape = ps.shape;
        shape.radius = 0.08f;
        if (currentPos.position.y > initYPos + 0.1)
        {
            main.gravityModifier = -0.2f;
            shape.position = new Vector3(0.0f, 0.25f, 0.0f);

        }
        else if (currentPos.position.y <= initYPos )
        {
            main.gravityModifier = 0.2f;
            shape.position = new Vector3(0.0f, -0.25f, 0.0f);
          
        }
	}
}
