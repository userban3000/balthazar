using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (MapGenerator))]
public class BalthazarGraphEditor : Editor {

    public override void OnInspectorGUI () {

        MapGenerator map = target as MapGenerator;

        DrawDefaultInspector();

        GUILayout.Space(20f);
        GUILayout.Label("Generation", EditorStyles.boldLabel);

        if ( GUILayout.Button("Generate Map" )) {
            map.GenerateMap();
        }

        if ( GUILayout.Button("Initial Node Setup" )) {
            map.MG_SetupNodes();
        }

        if ( GUILayout.Button("Lose Edges" )) {
            map.MG_LoseEdges();
        }

        if ( GUILayout.Button("Lose Nodes" )) {
            map.MG_LoseNodes();
        }

        if ( GUILayout.Button("Assign Starter Systems")) {
            map.MG_AssignStarterTeams(1);
        }

        GUILayout.Space(20f);
        GUILayout.Label("World", EditorStyles.boldLabel);

        
        if ( GUILayout.Button("Draw World" )) {
            map.MG_ConvertToWorldMap();
        }

        if ( GUILayout.Button("Clear World" )) {
            map.MG_Debug_ClearWorld();
        }

        if ( GUILayout.Button("Redraw World" )) {
            map.MG_Debug_ClearWorld();
            map.MG_ConvertToWorldMap();
        }

        GUILayout.Space(20f);
        GUILayout.Label("Testing & Debug", EditorStyles.boldLabel);

        if ( GUILayout.Button("Testing: Add Random Node" )) {
            map.AddNode();
        }

        if ( GUILayout.Button("Debug: Constellation Count" )) {
            map.MG_Debug_CC();
        }

        if ( GUILayout.Button("Debug: Show Graph Data" )) {
            map.ShowGraph();
        }

        if ( GUILayout.Button("Debug: Clear Graph Data" )) {
            map.MG_Debug_ClearGraph();
        }

        if ( GUILayout.Button("Debug: Clear Console" )) {
            map.ClearLog();
        }

        

        

        

    }

}
