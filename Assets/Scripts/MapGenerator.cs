using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BalthazarGraph;

public class MapGenerator : MonoBehaviour {
    
    Graph g = new Graph( new List<Node>(), new List<Node>() );

    private void Start() {
        g.GenerateFirstTriangle();

        g.AddNode();
        g.LinkNodes(3, 0, nodeDir.UR);

        g.DebugPrintNeighbors(0);
    }

}
