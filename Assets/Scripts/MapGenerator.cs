using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BalthazarGraph;

public class MapGenerator : MonoBehaviour {
    
    [Header("Seed")]
    public int seed;
    public bool randomSeed;

    [Header("Generation Settings")]
    public int nodesToAdd;
    public pickMode pick;

    Graph g;

    public void GenerateMap() {
        g = randomSeed ? new Graph(seed) : new Graph(Random.Range(-100000,100000));

        g.GenerateFirstTriangle();

        for ( int i = 0; i < nodesToAdd; i++ ) {
            g.MaterializePotentialNode(g.PickPotentialNode(pick));
        }

        g.DebugPrintGraph(true);
    }

    [ContextMenu("Add Another Node")]
    void AddNode() {
        g.MaterializePotentialNode(g.PickPotentialNode(pick));
    }

    [ContextMenu("Show Graph")]
    void ShowGraph() {
        g.DebugPrintGraph(true);
    }

    
}
