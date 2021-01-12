using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace US3K_Graph {

    public struct Node {
        public int nodeID;
        public List<Node> neighbors;
        public List<float> neighborCosts;

        //Empty node constructor
        public Node ( int _nID ) {
            nodeID = _nID;
            neighbors = new List<Node>();
            neighborCosts = new List<float>();
        }

        //Node constructor
        public Node ( int _nID, List<Node> _neighbors, List<float> _neighborCosts ) {
            nodeID = _nID;
            neighbors = _neighbors;
            neighborCosts = _neighborCosts;
        }

        //Adds an edge between this node and n, with a custom weight
        public void ConnectTo (Node n, float weight) {
            neighbors.Add(n);
            neighborCosts.Add(weight);

            n.neighbors.Add(this);
            n.neighborCosts.Add(weight);
        }

        //Adds an edge between this node and n, with a weight of 0
        public void ConnectTo (Node n) {
            neighbors.Add(n);
            neighborCosts.Add(0f);

            n.neighbors.Add(this);
            n.neighborCosts.Add(0f);
        }

    }

    public struct Graph {
        int nodeCount;
        public List<Node> nodes;

        //Graph constructor
        public Graph ( int _nodeCount, List<Node> _nodes ) {
            nodeCount = _nodeCount;
            nodes = _nodes;
        }

        //Adds a custom amount of nodes
        public void AddNodes(int nodesToAdd) {
            for ( int i = 0; i < nodesToAdd; i++ ) {
                Node n = new Node(nodeCount++);
                nodes.Add(n);
            }
        }

        //get a Node from an int ID
        public Node GetNode (int nodeID) {
            return nodes[nodeID];
        }

        //Links nodes by ID
        public void LinkNodes(int n1_ID, int n2_ID) {
            LinkNodes(GetNode(n1_ID), GetNode(n2_ID));
        }

        //Links nodes by ID, with weight
        public void LinkNodes(int n1_ID, int n2_ID, float weight) {
            LinkNodes(GetNode(n1_ID), GetNode(n2_ID), weight);
        }

        //Links nodes
        public void LinkNodes(Node n1, Node n2) {
            n1.ConnectTo(n2);
        }

        //Links nodes, with weight
        public void LinkNodes(Node n1, Node n2, float weight) {
            n1.ConnectTo(n2, weight);
        }

        //Debug function, prints the graph to console
        public void PrintGraph() {
            string graphText = null;
            for ( int i = 0; i < nodeCount; i++ ) {
                Node n = GetNode(i);
                graphText = graphText + n.nodeID.ToString() + ": ";
                foreach ( Node nCon in n.neighbors ) {
                    graphText = graphText + nCon.nodeID.ToString() + ", ";
                }
                Debug.Log(graphText);
                graphText = null;
            }
        }
    }

}