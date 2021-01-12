using System.Collections;
using System.Collections.Generic;
using US3K_Graph;
using UnityEngine;

public class MapGenerator : MonoBehaviour {
    
    Graph g = new Graph(0, new List<Node>());

    private void Start() {
        g.AddNodes(8);
        
        g.LinkNodes(0, 7);
        g.LinkNodes(1, 2);
        g.LinkNodes(1, 3);
        g.LinkNodes(1, 4);
        g.LinkNodes(1, 5);
        g.LinkNodes(2, 4);
        g.LinkNodes(2, 6);
        g.LinkNodes(3, 7);
        g.LinkNodes(5, 6);
        g.LinkNodes(6, 7);

        g.PrintGraph();
    }

}
