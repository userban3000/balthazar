using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (FriendlyUI))]
public class UIEditor : Editor {

    public override void OnInspectorGUI () {

        FriendlyUI fui = target as FriendlyUI;

        DrawDefaultInspector();

        GUILayout.Space(20f);
        GUILayout.Label("Debug Player Data", EditorStyles.boldLabel);

        if ( GUILayout.Button("Update Debug Sys" )) {
            fui.TargetSystem(fui.debugSystem);
        }

    }
}