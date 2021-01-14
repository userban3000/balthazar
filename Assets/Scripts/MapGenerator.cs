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

        g.MaterializePotentialNode(0, nodeDir.UL);
        g.MaterializePotentialNode(0, nodeDir.L);
        g.MaterializePotentialNode(0, nodeDir.DR);
        g.MaterializePotentialNode(0, nodeDir.DL);

        g.DebugPrintGraph();
    }

}
