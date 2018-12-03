using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerStatisticsUIController))]
public class PlayerStatisticsUIControllerEditor : Editor {
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        PlayerStatisticsUIController myScript = (PlayerStatisticsUIController)target;
        if (GUILayout.Button("Generate test player"))
        {
            myScript.Init(new Player()
            {
                NumDeaths = 3,
                NumKills = 50,
                NumStageWins = 10,
                ShotsFired = 5,
                SpecialsFired = 8,
                GravityChanges = 9
            });
        }
    }
}
