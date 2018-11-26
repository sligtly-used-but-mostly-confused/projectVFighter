using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelSelectManager))]
public class LevelSelectManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelSelectManager myScript = (LevelSelectManager)target;
        if (GUILayout.Button("Start Game"))
        {
            GameManager.Instance.DebugScene = true;
            GameManager.Instance.StartGame(myScript.LeadingLevels());
        }
    }
}
