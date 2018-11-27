using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TransitionController))]
public class TransitionControllerEditor : Editor {
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();


        TransitionController myScript = (TransitionController)target;

        if (GUILayout.Button("Start Transition"))
        {
            myScript.StartTransition(() => Debug.Log("done"));
        }
    }
}
