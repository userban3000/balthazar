using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//custom graph system for triangular diamond graphs
//based on US3K_Graph, which is also written by me
namespace BalthazarGraph {

    //up right, right, down right, down left, left, up left
    public enum nodeDir {UR, R, DR, DL, L, UL};

    public struct Node {
        public int nodeID;
        public bool[] hasNeighbor;
        public Node[] neighbors;
        public float[] neighborCosts;

        //opposite of direction
        public nodeDir Opposite(nodeDir direction) {
            return (int)direction < 3 ? ( direction + 3 ) : ( direction - 3 );
        }

        //Empty node constructor
        public Node ( int _nID ) {
            nodeID = _nID;
            hasNeighbor = new bool[6];
            neighbors = new Node[6];
            neighborCosts = new float[6];
        }

        //Node constructor
        public Node ( int _nID, bool[] _hasNeighbor, Node[] _neighbors, float[] _neighborCosts ) {
            nodeID = _nID;
            hasNeighbor = _hasNeighbor;
            neighbors = _neighbors;
            neighborCosts = _neighborCosts;
        }

        //Adds an edge between this node and n, with a custom weight
        public void ConnectTo (Node n, nodeDir direction, float weight) {
            hasNeighbor[(int)direction] = true;
            neighbors[(int)direction] = n;
            neighborCosts[(int)direction] = weight;

            nodeDir opposite = Opposite(direction);

            n.hasNeighbor[(int)opposite] = true;
            n.neighbors[(int)opposite] = this;
            n.neighborCosts[(int)opposite] = weight;
        }

        //Adds an edge between this node and n, with a weight of 0
        public void ConnectTo (Node n, nodeDir direction) {
            ConnectTo (n, direction, 0f);
        }

    }

    public struct Graph {
        public List<Node> nodes;
        public List<Node> potentialNodes;

        //Graph constructor
        public Graph (List<Node> _nodes, List<Node> _potentialNodes) {
            nodes = _nodes;
            potentialNodes = _potentialNodes;
        }

        //Adds a custom amount of nodes
        public void AddNodes(int nodesToAdd) {
            for ( int i = 0; i < nodesToAdd; i++ ) {
                AddNode();
            }
        }

        public void AddNode() {
            Node n = new Node(0);       //have to construct it with a parameter bcs parameterless gives errors. weird.
            n.nodeID = nodes.Count;     //i change the nodeID here anyways so it doesnt really matter.
            nodes.Add(n);
        }

        //generates the starting triangle of nodes
        public void GenerateFirstTriangle () {
            AddNodes(3);

            LinkNodes(0, 1, nodeDir.UR);
            LinkNodes(1, 2, nodeDir.DR);
            LinkNodes(2, 0, nodeDir.L);
        }

        //get a Node from an int ID
        public Node GetNodeFromID (int nodeID) {
            return nodes[nodeID];
        }

        //Links nodes by ID
        public void LinkNodes(int n1_ID, int n2_ID, nodeDir n2_RelativeTo_n1) {
            LinkNodes(GetNodeFromID(n1_ID), GetNodeFromID(n2_ID), n2_RelativeTo_n1);
        }

        //Links nodes by ID, with weight
        public void LinkNodes(int n1_ID, int n2_ID, nodeDir n2_RelativeTo_n1, float weight) {
            LinkNodes(GetNodeFromID(n1_ID), GetNodeFromID(n2_ID), n2_RelativeTo_n1, weight);
        }

        //Links nodes
        public void LinkNodes(Node n1, Node n2, nodeDir n2_RelativeTo_n1) {
            n1.ConnectTo(n2, n2_RelativeTo_n1);
        }

        //Links nodes, with weight
        public void LinkNodes(Node n1, Node n2, nodeDir n2_RelativeTo_n1, float weight) {
            n1.ConnectTo(n2, n2_RelativeTo_n1, weight);
        }

        //Prints the graph to console
        public void DebugPrintGraph() {
            string graphText = null;
            for ( int i = 0; i < nodes.Count; i++ ) {
                Node n = GetNodeFromID(i);
                graphText = graphText + n.nodeID.ToString() + ": ";
                foreach ( Node nCon in n.neighbors ) {
                    graphText = graphText + nCon.nodeID.ToString() + ", ";
                }
                Debug.Log(graphText);
                graphText = null;
            }
        }

        public void DebugPrintNeighbors(int nodeID) {
            Node n = GetNodeFromID(nodeID);

            string[] dirNames = nodeDir.GetNames(typeof(nodeDir));

            string nodeText = "Node: " + nodeID.ToString() + " | ";
            for ( int i = 0; i < 6; i++ ) {
                if ( n.hasNeighbor[i] ) {
                    nodeText += dirNames[i] + ": " + n.neighbors[i].nodeID.ToString() + ", ";
                }
            }
            Debug.Log(nodeText);
        }

        public void DebugPrintNeighbors(int nodeID, nodeDir dir) {
            Node n = GetNodeFromID(nodeID);

            string nodeText = "Node: " + nodeID.ToString() + " | " + dir + ": ";
            if ( n.hasNeighbor[(int)dir] )
                nodeText += n.nodeID.ToString();
            else
                nodeText += "No neighbor in this direction";
            Debug.Log(nodeText);
        }
    }

}
