using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BalthazarGraph;

public class MapGenerator : MonoBehaviour {
    
    Graph g = new Graph( new List<Node>(), new List<PotentialNode>() );

    private void Start() {
        g.GenerateFirstTriangle();

        g.AddNodes(4);

        g.LinkNodes(1, 3, nodeDir.UL);
        g.LinkNodes(3, 4, nodeDir.L);
        g.LinkNodes(4, 5, nodeDir.DL);
        g.LinkNodes(5, 6, nodeDir.DR);
        g.LinkNodes(6, 0, nodeDir.R);

        g.AddPotentialNode(0, nodeDir.UL);

        g.DebugPrintPnodeNeighbors(0);
    }

}
