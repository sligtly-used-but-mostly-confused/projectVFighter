using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    
    public static LevelManager Instance { get { return _instance; } }
    public List<GameObject> Walls;
    public float height;
    public float width;
    private static LevelManager _instance;

    private void Awake()
    {
        _instance = this;
    }

    void Start ()
    {
        height = Walls.Max(obj => obj.transform.localPosition.y) * 2;
        width = Walls.Max(obj => obj.transform.localPosition.x) * 2;
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
