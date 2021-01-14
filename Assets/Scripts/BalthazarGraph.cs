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

        public int NeighborCount() {
            int nc = 0;
            for ( int i = 0; i < 6; i++ ) {
                if ( hasNeighbor[i] )
                    nc++;
            }
            return nc;
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

        //equals operator
        public static bool operator ==(Node n1, Node n2) {
            return n1.nodeID == n2.nodeID;
        }

        //not equals operator
        public static bool operator !=(Node n1, Node n2) {
            return !(n1==n2);
        }

        //code to get rid of warnings on not overriding Object.Equals and Object.GetHashCode
        public override bool Equals(object obj)
        {
            if ( obj is Node node) {
                return this == node;
            }
            return false;
        }

        public override int GetHashCode() => new {nodeID}.GetHashCode();
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
    
        //equals operator
        //basically if its connected to all the same nodes
        public static bool operator == (PotentialNode n1, PotentialNode n2) {
            int match = 0;
            for ( int i = 0; i < 6; i++ ) {
                if ( n1.hasNeighbor[i] && n2.hasNeighbor[i] ) {
                    if ( n1.neighbors[i] == n2.neighbors[i] ) {
                        match++;
                    }
                } else if ( n1.hasNeighbor[i] == n2.hasNeighbor[i] ) {
                    match++;
                }
            }
            return match == 6;
        }

        //not equals operator
        public static bool operator != (PotentialNode n1, PotentialNode n2) {
            return !(n1==n2);
        }

        //code to get rid of warnings on not overriding Object.Equals and Object.GetHashCode
        public override bool Equals(object obj)
        {
            if ( obj is PotentialNode pNode) {
                return this == pNode;
            }
            return false;
        }

        public override int GetHashCode() => new {pNodeID}.GetHashCode();
    }

    public struct Graph {
        public List<Node> nodes;
        public List<PotentialNode> potentialNodes;
        private int nodeCount;

        //Graph constructor
        public Graph (List<Node> _nodes, List<PotentialNode> _potentialNodes) {
            nodes = _nodes;
            potentialNodes = _potentialNodes;
            nodeCount = 0;
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
            n.nodeID = nodeCount++;     //i change the nodeID here anyways so it doesnt really matter.
            nodes.Add(n);
        }

        //Adds a preconfigured node
        public void AddNode(Node n) {
            nodes.Add(n);
        }

        //deletes node by ID
        public void DeleteNode(int nodeID) {
            DeleteNode(GetNodeFromID(nodeID));
        }

        //deletes node by Node
        public void DeleteNode(Node n) {
            for ( int i = 0; i < 6; i++ ) {
                if ( n.hasNeighbor[i] == true ) {
                    DeleteEdge(n, n.neighbors[i]);
                }
            }
        }

        //fully removes node
        public void ShredNode(Node n) {
            nodes.Remove(n);
        }

        //delete edge between two nodes
        //keepn1 makes sure the first node isn't deleted, as to prevent recursion when this function gets called from DeleteNode()
        public void DeleteEdge(Node n1, Node n2) {
            bool foundEdge = false;
            int i;
            for ( i = 0; i < 6 && !foundEdge; i++ ) {
                if ( n1.neighbors[i] == n2 && n1.hasNeighbor[i] ) {
                    foundEdge = true;
                }
            }

            i--;

            if ( !foundEdge ) {
                Debug.LogError("Edge deletion between " + n1.nodeID.ToString() + " and " + n2.nodeID.ToString() + " failed! There is no such connection." );
                return;
            }
            
            n1.hasNeighbor[i] = false;
            n1.neighborCosts[i] = 0;
            if ( n1.NeighborCount() == 0 )
                ShredNode(n1);

            n2.hasNeighbor[(int)Opposite((nodeDir)i)] = false;
            n2.neighborCosts[(int)Opposite((nodeDir)i)] = 0;
            if ( n2.NeighborCount() == 0 )
                ShredNode(n2);
        }

        //delete edge between two nodes via int
        public void DeleteEdge(int n1_ID, int n2_ID) {
            DeleteEdge(GetNodeFromID(n1_ID), GetNodeFromID(n2_ID));
        }

        //Adds a new potential node
        public void AddPotentialNode(Node n, nodeDir dir) {
            PotentialNode pn = new PotentialNode(0);
            pn.pNodeID = potentialNodes.Count;
            pn.hasNeighbor[(int)Opposite(dir)] = true;
            pn.neighbors[(int)Opposite(dir)] = n;
            pn.HexPotentialNode();

            bool alreadyExists = false;
            foreach ( PotentialNode otherpn in potentialNodes ) {
                if ( otherpn == pn )
                    alreadyExists = true;
            }

            if ( alreadyExists ) {
                Debug.LogWarning("Tried to add a PotentialNode that already exists. The operation was canceled.");
                return;
            }

            potentialNodes.Add(pn);
        }

        //Adds a new potential node via nodeID
        public void AddPotentialNode(int nodeID, nodeDir dir) {
            AddPotentialNode(GetNodeFromID(nodeID), dir);
        }

        //generates the starting triangle of nodes
        public void GenerateFirstTriangle () {
            AddNodes(3);

            LinkNodes(0, 1, nodeDir.UR);
            LinkNodes(1, 2, nodeDir.DR);
            LinkNodes(2, 0, nodeDir.L);

            AddPotentialNode(0, nodeDir.UL);
            AddPotentialNode(1, nodeDir.R);
            AddPotentialNode(2, nodeDir.DL);
        }

        //finds a potential node from a given nodeID and direction
        public int FindPotentialNode(int nodeID, nodeDir dir) {
            return FindPotentialNode(GetNodeFromID(nodeID), dir);
        }

        //finds a potential node from a given node and direction
        //returns its position in the potentialNodes list
        public int FindPotentialNode(Node n, nodeDir dir) {
            bool found = false;
            int k;

            for ( k = 0; k < potentialNodes.Count && !found; k++ ) {
                found = potentialNodes[k].neighbors[(int)Opposite(dir)] == n && potentialNodes[k].hasNeighbor[(int)Opposite(dir)];
            }

            if (!found) {
                string[] dirNames = nodeDir.GetNames(typeof(nodeDir));
                Debug.LogError("FATAL ERROR: there is no Potential Node starting from " + n.nodeID + ", going " + dirNames[(int)dir] + ". No new nodes have been created. Code: PNODE_NOT_FOUND" );
                Debug.LogWarning("The error above can result in cascading errors. Fix before attempting to generate Graph.");
                return -1;
            }

            return k - 1;
        }

        //turns a potential node into a node found by ID
        public void MaterializePotentialNode(int nodeID, nodeDir dir) {
            MaterializePotentialNode(GetNodeFromID(nodeID), dir);
        }

        //turns a potential node into a node
        public void MaterializePotentialNode(Node n, nodeDir dir) {
            int pnIndex = FindPotentialNode(n, dir);
            if ( pnIndex == -1 ) {
                return;
            }
            PotentialNode pn = potentialNodes[pnIndex];
            potentialNodes.Remove(pn);

            Node newNode = new Node(nodeCount++);

            int fpn, lpn;
            fpn = lpn = -1;

            for ( int i = 0; i < 6; i++ ) {
                if ( pn.hasNeighbor[i] ) {
                    LinkNodes(newNode, pn.neighbors[i], (nodeDir)i );
                    if ( !pn.hasNeighbor[(int)Next((nodeDir)i)] ) {
                        lpn = (int)Next((nodeDir)i);
                    }
                    if ( fpn == -1 ) {
                        if ( !pn.hasNeighbor[(int)Prev((nodeDir)i)] ) {
                            fpn = (int)Prev((nodeDir)i);
                        }
                    }
                }
            }

            AddNode(newNode);

            foreach ( PotentialNode allPn in potentialNodes ) {
                allPn.HexPotentialNode();
            }

            if ( fpn != -1 )
                AddPotentialNode(newNode, (nodeDir)fpn);
            if ( lpn != -1 )
                AddPotentialNode(newNode, (nodeDir)lpn);
        }

        //get a Node from an int ID
        public Node GetNodeFromID (int nodeID) {

            foreach ( Node n in nodes ) {
                if ( n.nodeID == nodeID )
                    return n;
            } 

            Debug.LogError("Node with ID " + nodeID + " does not exist in the current Graph. Any Nodes that appear as '-1' are a cause of this.");
            return new Node(-1);
        }

        //get a PotentialNode from an int ID
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

        //prev direction required to run a full hexagon backwards (ccw)
        public nodeDir Prev(nodeDir direction) {
            return (int)direction < 1 ? ( direction + 5 ) : ( direction - 1 );
        }

        //converts an outward direction from an inner node to the direction it has to start following for traversing all of the nodes in its hex
        public nodeDir OutToSideways(nodeDir direction) {
            return (int)direction < 4 ? ( direction + 2 ) : ( direction - 4 );
        }

        //Prints the graph to console with no pnodes by default
        public void DebugPrintGraph() {
            DebugPrintGraph(false);
        }

        //Prints the graph to console
        public void DebugPrintGraph(bool inclPotential) {
            
            Debug.Log("===== PRINTING GRAPH =====");

            foreach ( Node n in nodes )
                DebugPrintNeighbors(n);

            if ( inclPotential ) {
                foreach ( PotentialNode pn in potentialNodes ) {
                    DebugPrintPnodeNeighbors(pn);
                }
            }
            Debug.Log("=== END PRINTING GRAPH ===");
        }

        //Prints all of a node's neighbors to console
        public void DebugPrintPnodeNeighbors(PotentialNode pn) {

            string[] dirNames = nodeDir.GetNames(typeof(nodeDir));

            string nodeText = "Potential Node | ";
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

            if ( n.nodeID == -1 ) 
                return;
            else
                DebugPrintNeighbors(n);
        }

        //Prints all of a node's neighbors to console
        public void DebugPrintNeighbors(Node n) {
            string[] dirNames = nodeDir.GetNames(typeof(nodeDir));

            string nodeText = "Node: " + n.nodeID.ToString() + " | ";
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
