using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ControllerSelectManager))]
public class ControllerSelectManagerEditor : Editor {
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        ControllerSelectManager myScript = (ControllerSelectManager)target;
        if (GUILayout.Button("add new keyboard player"))
        {
            myScript.SpawnPlayer(MappedInput.InputDevices[2]);
        }
    }
}
