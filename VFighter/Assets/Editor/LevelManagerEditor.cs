using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        if (GUILayout.Button("Load Next Level"))
        {
            GameManager.Instance.DebugScene = true;
            GameManager.Instance.LoadNextStage();
        }
    }
}
