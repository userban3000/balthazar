using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (MapGenerator))]
public class BalthazarGraphEditor : Editor {

    public override void OnInspectorGUI () {

        MapGenerator map = target as MapGenerator;

        DrawDefaultInspector();

        if ( GUILayout.Button("DISABLED: Generate Graph" )) {
            map.GenerateMap();
        }

        if ( GUILayout.Button("Initial Node Setup" )) {
            map.MG_SetupNodes();
        }

        if ( GUILayout.Button("Show Graph Data" )) {
            map.ShowGraph();
        }

        if ( GUILayout.Button("Convert to World" )) {
            map.MG_ConvertToWorldMap();
        }

        if ( GUILayout.Button("Add Node" )) {
            map.AddNode();
        }

        if ( GUILayout.Button("Debug: Clear Graph" )) {
            map.MG_Debug_ClearGraph();
        }

        if ( GUILayout.Button("Debug: Clear World" )) {
            map.MG_Debug_ClearWorld();
        }

        if ( GUILayout.Button("WIP: Lose Edges" )) {
            map.MG_LoseEdges();
        }

        if ( GUILayout.Button("Clear Console" )) {
            map.ClearLog();
        }

    }

}
