using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (MapGenerator))]
public class BalthazarGraphEditor : Editor {

    public override void OnInspectorGUI () {

        MapGenerator map = target as MapGenerator;

        if ( DrawDefaultInspector() || GUILayout.Button("Generate Graph" )) {
            Debug.ClearDeveloperConsole();
            map.GenerateMap();
        }
    }

}
