using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InGameMenuUIEditor : Editor {
    public SerializedProperty MainMenu;
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        /*
        if (GUILayout.Button("Load Next Level"))
        {
            GameManager.Instance.DebugScene = true;
            GameManager.Instance.LoadNextStage();
        }
        */
        EditorGUILayout.PropertyField(MainMenu, new GUIContent("Scene"));
        serializedObject.ApplyModifiedProperties();
        
    }
}
