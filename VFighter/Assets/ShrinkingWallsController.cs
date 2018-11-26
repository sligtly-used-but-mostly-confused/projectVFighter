using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkingWallsController : MonoBehaviour {

    [System.Serializable]
    public struct WallPoint
    {
        public Vector3 startLocation;
        public Vector3 endLocation;

        public WallPoint(GameObject s, GameObject f){
            startLocation = s.transform.position;
            endLocation = f.transform.position;
        }
    }

    [System.Serializable]
    public struct WallPointGameObjects
    {
        public GameObject startPoint;
        public GameObject endPoint;
    }

    public GameObject wallPrefab;
    public GameObject pointMarker;

    public float timeToShrink;
    [SerializeField]
    private List<WallPointGameObjects> wallPointGameObjects = new List<WallPointGameObjects>();

    private List<WallPoint> wallPoints = new List<WallPoint>();
    private List<GameObject> walls = new List<GameObject>();
    private float startTime;

	void Start () {
        //set the wall points to whatever is in WallpointGameObjects
        foreach(WallPointGameObjects wpgo in wallPointGameObjects){
            wallPoints.Add(new WallPoint(wpgo.startPoint, wpgo.endPoint));
        }

        //grab the start time
        startTime = Time.time;

        //spawn in the walls
        for (int i = 0; i < wallPoints.Count; ++i){
            WallPoint firstWallPoint = wallPoints[i];
            WallPoint secondWallPoint = wallPoints[(i + 1) % wallPoints.Count];
            GameObject wall = Instantiate(wallPrefab,transform);
            UpdateWallToPoints(wall, firstWallPoint.startLocation, secondWallPoint.startLocation);
            walls.Add(wall);
        }
	}
	
	void Update () {
        //get the fraction of time to total time
        float passedTime = Time.time - startTime;
        float percentageComplete = passedTime / timeToShrink;
        if (passedTime > timeToShrink) percentageComplete = 1;

        //move the walls
        for (int i = 0; i < wallPoints.Count; ++i)
        {
            WallPoint firstWallPoint = wallPoints[i];
            WallPoint secondWallPoint = wallPoints[(i + 1) % wallPoints.Count];
            Vector3 firstPoint = GetCurrentLocation(firstWallPoint, percentageComplete);
            Vector3 secondPoint = GetCurrentLocation(secondWallPoint, percentageComplete);
            UpdateWallToPoints(walls[i], firstPoint, secondPoint);
        }

    }

    void UpdateWallToPoints(GameObject wall, Vector3 point1, Vector3 point2){
        //set scale
        wall.transform.localScale = new Vector3(Vector3.Distance(point1, point2), 0.5f, 0.5f);

        //set location
        wall.transform.position = new Vector3((point1.x + point2.x) / 2 , (point1.y + point2.y) / 2 , 0);

        //set rotation
        wall.transform.rotation = Quaternion.Euler(wall.transform.rotation.x, wall.transform.rotation.y, Mathf.Rad2Deg * Mathf.Atan2(point2.y - point1.y, point2.x - point1.x));
    }

    Vector3 GetCurrentLocation(WallPoint wp, float percentage)
    {
        return Vector3.Lerp(wp.startLocation, wp.endLocation, percentage);
    }

}
