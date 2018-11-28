using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ShrinkingWallsController : MonoBehaviour {

    [System.Serializable]
    public struct WallPoint
    {
        public Vector3 startLocation;
        public Vector3 endLocation;

        public WallPoint(GameObject s, GameObject f)
        {
            startLocation = s.transform.position;
            endLocation = f.transform.position;
        }
    }

    public GameObject wallPrefab;
    public GameObject pointMarker;

    public float cycleLength;
    public GameObject startPoints;
    public GameObject endPoints;

    private List<WallPoint> wallPoints = new List<WallPoint>();
    private List<GameObject> walls = new List<GameObject>();
    private float startTime;

    const float pi = 3.1415f;
    private float frequency; // Frequency in Hz

    void Start () {
        //set the wall points to whatever is input
        List<RectTransform> startTransforms = startPoints.GetComponentsInChildren<RectTransform>().ToList();
        List<RectTransform> finishTransfroms = endPoints.GetComponentsInChildren<RectTransform>().ToList();

        Debug.Assert(startTransforms.Count == finishTransfroms.Count);

        // -1 for the first point, which is the root
        int numPoints = startTransforms.Count - 1;

        for (int i = 1; i < startTransforms.Count; ++i){
            wallPoints.Add(new WallPoint(startTransforms[i].gameObject, finishTransfroms[i].gameObject));
        }

        //set frequency
        frequency = 1/cycleLength; 

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
        float percentageComplete = Pulse(passedTime);

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
        wall.transform.localScale = new Vector3(Vector3.Distance(point1, point2), 0.7f, 0.7f);

        //set location
        wall.transform.position = new Vector3((point1.x + point2.x) / 2 , (point1.y + point2.y) / 2 , 0);

        //set rotation
        wall.transform.rotation = Quaternion.Euler(wall.transform.rotation.x, wall.transform.rotation.y, Mathf.Rad2Deg * Mathf.Atan2(point2.y - point1.y, point2.x - point1.x));
    }

    Vector3 GetCurrentLocation(WallPoint wp, float percentage)
    {
        return Vector3.Lerp(wp.startLocation, wp.endLocation, percentage);
    }

    //taken from https://stackoverflow.com/questions/3018550/how-to-create-pulsating-value-from-0-1-0-1-0-etc-for-a-given-duration
    float Pulse(float time)
    {
        return 0.5f * (1 + Mathf.Sin(2 * pi * frequency * time)) *GameManager.Instance.TimeScale;
    }

}
