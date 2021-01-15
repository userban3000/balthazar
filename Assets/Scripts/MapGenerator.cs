using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BalthazarGraph;

public class MapGenerator : MonoBehaviour {
    
    Graph g = new Graph( new List<Node>(), new List<PotentialNode>() );

    private void Start() {
        GenerateMap();
    }

    public void GenerateMap() {
        g = new Graph( new List<Node>(), new List<PotentialNode>() );

        g.GenerateFirstTriangle();

        g.DebugPrintGraph(true);
    }

}
