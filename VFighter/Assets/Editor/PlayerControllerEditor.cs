using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(KeyboardPlayerController))]
public class PlayerControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        KeyboardPlayerController player = (KeyboardPlayerController) target;
        if (GUILayout.Button("Drop Player"))
        {
            player.DropPlayer();
        }
    }
}
