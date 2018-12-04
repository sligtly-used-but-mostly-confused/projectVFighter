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
    public float wallWidth = 0.7f;
    public float cornerSize = 1f;
    public GameObject startPoints;
    public GameObject endPoints;

    private List<WallPoint> wallPoints = new List<WallPoint>();
    private List<GameObject> walls = new List<GameObject>();
    private List<GameObject> corners = new List<GameObject>();
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

        //spawn in the walls and corners
        for (int i = 0; i < wallPoints.Count; ++i){
            WallPoint firstWallPoint = wallPoints[i];
            WallPoint secondWallPoint = wallPoints[(i + 1) % wallPoints.Count];
            GameObject wall = Instantiate(wallPrefab,transform);
            UpdateWallToPoints(wall, firstWallPoint.startLocation, secondWallPoint.startLocation);
            walls.Add(wall);

            //corner
            GameObject corner = Instantiate(wallPrefab, transform);
            UpdateCornerToPoint(corner, firstWallPoint.startLocation);
            corners.Add(corner);
        }

        StartCoroutine(MoveWalls());
	}
	
    IEnumerator MoveWalls() {
        float passedTime = 0;
        while (true)
        {
            //get the fraction of time to total time
            passedTime += Time.deltaTime * GameManager.Instance.TimeScale;
            float percentageComplete = Pulse(passedTime);
            for (int i = 0; i < wallPoints.Count; ++i)
            {
                //move the walls
                WallPoint firstWallPoint = wallPoints[i];
                WallPoint secondWallPoint = wallPoints[(i + 1) % wallPoints.Count];
                Vector3 firstPoint = GetCurrentLocation(firstWallPoint, percentageComplete);
                Vector3 secondPoint = GetCurrentLocation(secondWallPoint, percentageComplete);
                UpdateWallToPoints(walls[i], firstPoint, secondPoint);

                //move the corners
                UpdateCornerToPoint(corners[i], firstPoint);
            }


            yield return null;
        }
    }

    void UpdateWallToPoints(GameObject wall, Vector3 point1, Vector3 point2){
        //set scale
        wall.transform.localScale = new Vector3(Vector3.Distance(point1, point2), wallWidth, wallWidth);
        //set location
        wall.transform.position = new Vector3((point1.x + point2.x) / 2 , (point1.y + point2.y) / 2 , 0);
        //set rotation
        wall.transform.rotation = Quaternion.Euler(wall.transform.rotation.x, wall.transform.rotation.y, Mathf.Rad2Deg * Mathf.Atan2(point2.y - point1.y, point2.x - point1.x));
    }

    void UpdateCornerToPoint(GameObject corner, Vector3 position){
        //set scale
        corner.transform.localScale = new Vector3(cornerSize,cornerSize,cornerSize);
        //set location
        corner.transform.position = position;
        //set rotation
        corner.transform.rotation = Quaternion.Euler(0f,0f,0f);
    }

    Vector3 GetCurrentLocation(WallPoint wp, float percentage)
    {
        return Vector3.Lerp(wp.startLocation, wp.endLocation, percentage);
    }

    //taken from https://stackoverflow.com/questions/3018550/how-to-create-pulsating-value-from-0-1-0-1-0-etc-for-a-given-duration
    float Pulse(float time)
    {
        return 0.5f * (1 + Mathf.Sin(2 * pi * frequency * time));
    }

}
