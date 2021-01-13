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

    public struct PotentialNode {
        public int pNodeID;
        public int neighborCount;
        public bool[] hasNeighbor;
        public Node[] neighbors;

        //PotentialNode constructor
        public PotentialNode ( int _pnID ) {
            pNodeID = _pnID;
            neighborCount = 0;
            hasNeighbor = new bool[6];
            neighbors = new Node[6];
        }

        //updates neighbor count for easier checking
        public void UpdateNeighborCount() {
            neighborCount = 0;
            for ( int i = 0; i < 6; i++ ) {
                if ( hasNeighbor[i] ) {
                    neighborCount++;
                }
            }
        }

        //next direction required to run a full hexagon
        public nodeDir Next(nodeDir direction) {
            return (int)direction < 5 ? ( direction + 1 ) : ( direction - 5 );
        }

        //prev direction required to run a full hexagon
        public nodeDir Prev(nodeDir direction) {
            return (int)direction < 1 ? ( direction + 5 ) : ( direction - 1 );
        }

        //converts an outward direction from an inner node to the direction it has to start following for traversing all of the nodes in its hex
        public nodeDir OutToSideways(nodeDir direction) {
            return (int)direction < 4 ? ( direction + 2 ) : ( direction - 4 );
        }

        //opposite of direction
        public nodeDir Opposite(nodeDir direction) {
            return (int)direction < 3 ? ( direction + 3 ) : ( direction - 3 );
        }

        //updates to reflect all nodes in a hex around the given pNode
        public void HexPotentialNode() {
            bool[] visited = new bool[6];
            int nodesAroundPNode = 0;
            for ( int k = 0; k < 6; k++ ) {
                int i = k;
                if ( hasNeighbor[i] ) {
                    visited[i] = true;
                    nodesAroundPNode++;

                    string[] dirNames = nodeDir.GetNames(typeof(nodeDir));

                    Node visitedNode = neighbors[i];
                    nodeDir nextDir = OutToSideways((nodeDir)i);
                    nodeDir prevDir = Opposite(Prev(nextDir));

                    bool nextExists = visitedNode.hasNeighbor[(int)nextDir];
                    while ( nextExists ) {
                        i = i == 5 ? ( 0 ) : ( i + 1 );
                        if ( !visited[i] ) {
                            visitedNode = visitedNode.neighbors[(int)nextDir];
                            nodesAroundPNode++;
                            hasNeighbor[i] = true;
                            neighbors[i] = visitedNode;
                            visited[i] = true;
                        }
                        nextDir = Next(nextDir);
                        nextExists = visitedNode.hasNeighbor[(int)nextDir];
                    }

                    i = k;
                    visitedNode = neighbors[i];

                    bool prevExists = visitedNode.hasNeighbor[(int)prevDir];
                    while ( prevExists ) {
                        i = i == 0 ? ( 5 ) : ( i - 1 );
                        if ( !visited[i] ) {
                            visitedNode = visitedNode.neighbors[(int)prevDir];
                            nodesAroundPNode++;
                            hasNeighbor[i] = true;
                            neighbors[i] = visitedNode;
                            visited[i] = true;
                        }
                        prevDir = Prev(prevDir);
                        prevExists = visitedNode.hasNeighbor[(int)prevDir];
                    }
                }
            }
            neighborCount = nodesAroundPNode;
        }
    }

    public struct Graph {
        public List<Node> nodes;
        public List<PotentialNode> potentialNodes;

        //Graph constructor
        public Graph (List<Node> _nodes, List<PotentialNode> _potentialNodes) {
            nodes = _nodes;
            potentialNodes = _potentialNodes;
        }

        //Adds a custom amount of nodes
        public void AddNodes(int nodesToAdd) {
            for ( int i = 0; i < nodesToAdd; i++ ) {
                AddNode();
            }
        }

        //Adds one new node
        public void AddNode() {
            Node n = new Node(0);       //have to construct it with a parameter bcs parameterless gives errors. weird.
            n.nodeID = nodes.Count;     //i change the nodeID here anyways so it doesnt really matter.
            nodes.Add(n);
        }

        //Adds a new potential node
        //TODO: check if pnode already exists
        public void AddPotentialNode(Node n, nodeDir dir) {
            PotentialNode pn = new PotentialNode(0);
            pn.pNodeID = potentialNodes.Count;
            pn.hasNeighbor[(int)Opposite(dir)] = true;
            pn.neighbors[(int)Opposite(dir)] = n;
            pn.HexPotentialNode();
            potentialNodes.Add(pn);
        }

        //Adds a new potential node
        //TODO: check if pnode already exists
        public void AddPotentialNode(int nodeID, nodeDir dir) {
            AddPotentialNode(GetNodeFromID(nodeID), dir);
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

        public PotentialNode GetPotentialNodeFromID(int pnodeID) {
            return potentialNodes[pnodeID];
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

        //opposite of direction
        public nodeDir Opposite(nodeDir direction) {
            return (int)direction < 3 ? ( direction + 3 ) : ( direction - 3 );
        }

        //next direction required to run a full hexagon
        public nodeDir Next(nodeDir direction) {
            return (int)direction < 5 ? ( direction + 1 ) : ( direction - 5 );
        }

        //converts an outward direction from an inner node to the direction it has to start following for traversing all of the nodes in its hex
        public nodeDir OutToSideways(nodeDir direction) {
            return (int)direction < 4 ? ( direction + 2 ) : ( direction - 4 );
        }

        //Connects a p-node to all nodes on it hex, then returns how many nodes are there
        public void HexPotentialNode(int pNodeID) {
            PotentialNode pn = GetPotentialNodeFromID(pNodeID);
            bool[] visited = new bool[6];
            int nodesAroundPNode = 0;
            for ( int i = 0; i < 6; i++ ) {
                if ( pn.hasNeighbor[i] ) {
                    visited[i] = true;
                    nodesAroundPNode++;

                    Node visitedNode = pn.neighbors[i];
                    nodeDir nextDir = OutToSideways((nodeDir)i);
                    bool nextExists = visitedNode.hasNeighbor[(int)nextDir];

                    while ( nextExists ) {
                        nodesAroundPNode++;
                        visitedNode = visitedNode.neighbors[(int)nextDir];
                        if ( !visited[++i] ) {
                            pn.hasNeighbor[i] = true;
                            pn.neighbors[i] = visitedNode;
                            visited[i] = true;
                        }
                        nextDir = Next(nextDir);
                        nextExists = visitedNode.hasNeighbor[(int)nextDir];
                    }
                }
            }
            pn.neighborCount = nodesAroundPNode;
            //theres gotta be a better way to do this
            potentialNodes[pNodeID] = pn;
        }

        //Prints the graph to console
        public void DebugPrintGraph() {
            string graphText = null;
            for ( int i = 0; i < nodes.Count; i++ ) {
                Node n = GetNodeFromID(i);
                graphText = graphText + n.nodeID.ToString() + ": ";
                for ( int j = 0; j < 6; j++ ) {
                    if ( n.hasNeighbor[j] )
                        graphText = graphText + n.neighbors[j].nodeID.ToString() + ", ";
                }
                Debug.Log(graphText);
                graphText = null;
            }
        }

        //Prints all of a node's neighbors to console
        public void DebugPrintPnodeNeighbors(int pnodeID) {
            PotentialNode pn = GetPotentialNodeFromID(pnodeID);

            string[] dirNames = nodeDir.GetNames(typeof(nodeDir));

            string nodeText = "Potential Node: " + pnodeID.ToString() + " | ";
            for ( int i = 0; i < 6; i++ ) {
                if ( pn.hasNeighbor[i] ) {
                    nodeText += dirNames[i] + ": " + pn.neighbors[i].nodeID.ToString() + ", ";
                }
            }
            Debug.Log(nodeText);
        }

        //Prints all of a node's neighbors to console
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

        //Prints the ID of a neighbor in a certain direction, if it exists
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
