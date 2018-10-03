using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class LevelSelectManager : MonoBehaviour {

    public List<string> levels;
    public GameObject platform;
    public GameObject levelZone;

    private List<LevelZoneController> zones = new List<LevelZoneController>();

    // Use this for initialization
    void Start() {
        SpawnLevelPlatforms();

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void SpawnLevelPlatforms(){

        int count = levels.Count;
        int levelsRemaining = count;
        int levelIndex = 0;

        if (count > 4)
        { // 6 and 5
            for (int x = -4; x <= 4; x += 4)
            {
                Vector3 pos = new Vector3(x, 0, 0);
                Instantiate(platform, pos, Quaternion.identity);

                if (levelsRemaining > 1)
                {
                    //instantiate the levels


                    for (int y = -1; y < 2; y += 2){
                        Vector3 zonePos = pos;
                        zonePos += new Vector3(0, y * 1.25f, 0);
                        LevelZoneController zone = Instantiate(levelZone, zonePos, Quaternion.identity).GetComponent<LevelZoneController>();
                        zones.Add(zone);
                        zone.levelName = levels[levelIndex];
                        ++levelIndex;
                        --levelsRemaining;
                    }

                    //set the level names
                    //put them in zones list
                    //update variables
                }
                else if (levelsRemaining == 1){
                    Vector3 zonePos = pos;
                    zonePos += new Vector3(0, 1.25f, 0);
                    LevelZoneController zone = Instantiate(levelZone, zonePos, Quaternion.identity).GetComponent<LevelZoneController>();
                    zones.Add(zone);
                    zone.levelName = levels[levelIndex];
                    ++levelIndex;
                    --levelsRemaining;
                }

            }
        }
        else
        { //1-4
            for (int x = -4; x <= 4; x += 8)
            {
                Vector3 pos = new Vector3(x, 0, 0);
                Instantiate(platform, pos, Quaternion.identity);
            }
        }
    }

}
