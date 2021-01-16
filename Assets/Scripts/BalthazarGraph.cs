using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

//custom graph system for triangular diamond graphs
//based on US3K_Graph, which is also written by me
namespace BalthazarGraph {

//===================================================ENUMS===================================================

    //up right, right, down right, down left, left, up left
    public enum nodeDir {UR, R, DR, DL, L, UL};
    public enum pickMode {Structural, SemiStructural, Random, SemiChaotic, Chaotic};

//===================================================COORD===================================================

    public struct Coord {
        public int x;
        public int y;

        public Coord (int _x, int _y) {
            x = _x;
            y = _y;
        }

        //predefined Coords
        public static readonly Coord UR = new Coord (1, 1);
        public static readonly Coord R = new Coord (2, 0);
        public static readonly Coord DR = new Coord (1, -1);
        public static readonly Coord DL = new Coord (-1, -1);
        public static readonly Coord L = new Coord (-2, 0);
        public static readonly Coord UL = new Coord (-1, 1);
        public static readonly Coord Zero = new Coord (0, 0);

        //coord addition
        public static Coord operator + (Coord c1, Coord c2) {
            return new Coord(c1.x + c2.x, c1.y + c2.y);
        }

        //equals operator
        //basically if its connected to all the same nodes
        public static bool operator == (Coord c1, Coord c2) {
            return c1.x == c2.x && c1.y == c2.y;
        }

        //not equals operator
        public static bool operator != (Coord c1, Coord c2) {
            return !(c1==c2);
        }

        //code to get rid of warnings on not overriding Object.Equals and Object.GetHashCode
        public override bool Equals(object obj)
        {
            if ( obj is Coord c) {
                return this == c;
            }
            return false;
        }

        public override int GetHashCode() => new {x,y}.GetHashCode();

    }

//===================================================NODE===================================================

    public struct Node {
        public int nodeID;
        public Coord coord;
        public bool[] hasNeighbor;
        public Node[] neighbors;
        public float[] neighborCosts;

        //get coord delta from a nodedir
        public Coord CoordFromNodeDir (nodeDir dir) {
            switch (dir) {
                case nodeDir.UR:
                    return Coord.UR;
                case nodeDir.R:
                    return Coord.R; 
                case nodeDir.DR:
                    return Coord.DR;
                case nodeDir.DL:
                    return Coord.DL;
                case nodeDir.L:
                    return Coord.L;
                case nodeDir.UL:
                    return Coord.UL;
                default:
                    return Coord.Zero;
            }
        }

        //opposite of direction
        public nodeDir Opposite(nodeDir direction) {
            return (int)direction < 3 ? ( direction + 3 ) : ( direction - 3 );
        }

        //Empty node constructor
        public Node ( int _nID ) {
            nodeID = _nID;
            coord = new Coord(0,0);
            hasNeighbor = new bool[6];
            neighbors = new Node[6];
            neighborCosts = new float[6];
        }

        //Node constructor
        public Node ( int _nID, Coord _coord, bool[] _hasNeighbor, Node[] _neighbors, float[] _neighborCosts ) {
            nodeID = _nID;
            coord = _coord;
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

            n.coord = this.coord + CoordFromNodeDir(direction);
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

//===================================================POTENTIAL NODE===================================================

    public struct PotentialNode {
        public Coord coord;
        public int neighborCount;
        public bool[] hasNeighbor;
        public Node[] neighbors;

        //get coord delta from a nodedir
        public Coord CoordFromNodeDir (nodeDir dir) {
            switch (dir) {
                case nodeDir.UR:
                    return Coord.UR;
                case nodeDir.R:
                    return Coord.R; 
                case nodeDir.DR:
                    return Coord.DR;
                case nodeDir.DL:
                    return Coord.DL;
                case nodeDir.L:
                    return Coord.L;
                case nodeDir.UL:
                    return Coord.UL;
                default:
                    return Coord.Zero;
            }
        }

        public PotentialNode ( int discard ) {
            coord = new Coord(0,0);
            neighborCount = 0;
            hasNeighbor = new bool[6];
            neighbors = new Node[6];
        }

        //updates neighbor count for easier checking
        public int GetNeighborCount() {
            int neighborCount = 0;
            for ( int i = 0; i < 6; i++ ) {
                if ( hasNeighbor[i] ) {
                    neighborCount++;
                }
            }
            return neighborCount;
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
    
        //equals operator
        //basically if its connected to all the same nodes
        public static bool operator == (PotentialNode n1, PotentialNode n2) {
            return n1.coord == n2.coord;
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

        public override int GetHashCode() => new {coord}.GetHashCode();
    }

//===================================================GRAPH===================================================

    public struct Graph {
        public List<Node> nodes;
        public List<PotentialNode> potentialNodes;
        private int nodeCount;

        public Coord CoordFromNodeDir (nodeDir dir) {
            switch (dir) {
                case nodeDir.UR:
                    return Coord.UR;
                case nodeDir.R:
                    return Coord.R; 
                case nodeDir.DR:
                    return Coord.DR;
                case nodeDir.DL:
                    return Coord.DL;
                case nodeDir.L:
                    return Coord.L;
                case nodeDir.UL:
                    return Coord.UL;
                default:
                    return Coord.Zero;
            }
        }

        //Graph constructor
        //set the seed to 0 for a random one
        public Graph (int seed) {
            Random.InitState(seed);
            nodes = new List<Node>();
            potentialNodes = new List<PotentialNode>();
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
            Node n = new Node(0);
            n.nodeID = nodeCount++;
            nodes.Add(n);
        }

        //Adds a preconfigured node
        public void AddNode(Node n) {
            nodes.Add(n);
        }

        //deletes node by ID
        public void DeleteNode(int nodeID) {
            DeleteNode(nodes[nodeID]);
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
            DeleteEdge(nodes[n1_ID], nodes[n2_ID]);
        }

        //merges two conflicting potential nodes
        public PotentialNode MergePotentialNodes(PotentialNode mergeThis, PotentialNode intoThis) {
            PotentialNode pn = intoThis;
            for ( int i = 0; i < 6; i++ ) {
                if ( mergeThis.hasNeighbor[i] ) {
                    pn.hasNeighbor[i] = true;
                    pn.neighbors[i] = mergeThis.neighbors[i];
                }
            }
            return pn;
        }

        public bool FindNodeAtCoord(Coord c, out Node foundNode ) {
            foundNode = new Node(-1);
            foreach ( Node n in nodes ) {
                if ( n.coord == c ) {
                    foundNode = n;
                }
            }
            return foundNode.nodeID != -1;
        }

        //updates a potential node with its neighbors
        public PotentialNode HexPotentialNode(PotentialNode pn) {
            pn.neighborCount = 0;
            for ( int i = 0; i < 6; i++ ) {
                Coord check = pn.coord + CoordFromNodeDir((nodeDir)i);
                if ( FindNodeAtCoord(check, out Node n) ) {
                    pn.hasNeighbor[i] = true;
                    pn.neighborCount++;
                    pn.neighbors[i] = n;
                }
            }
            return pn;
        }

        //Adds a new potential node
        public void AddPotentialNode(Node n, nodeDir dir) {
            PotentialNode pn = new PotentialNode(0);
            pn.coord = n.coord + CoordFromNodeDir(dir);
            
            //Debug.Log(pn.coord.x.ToString() + " " + pn.coord.y.ToString());
            pn = HexPotentialNode(pn);

            bool alreadyExists = false;
            for ( int i = 0; i < potentialNodes.Count; i++ ) {
                if ( pn == potentialNodes[i] ) {
                    potentialNodes[i] = MergePotentialNodes(pn, potentialNodes[i]);
                    alreadyExists = true;
                }
            }

            if ( !alreadyExists )
                potentialNodes.Add(pn);
        }

        //Adds a new potential node via nodeID
        public void AddPotentialNode(int nodeID, nodeDir dir) {
            AddPotentialNode(nodes[nodeID], dir);
        }

        //generates the starting triangle of nodes
        public void GenerateFirstTriangle () {
            AddNodes(3);

            LinkNodes(0, 1, nodeDir.UR);
            LinkNodes(1, 2, nodeDir.DR);
            LinkNodes(0, 2, nodeDir.R);

            AddPotentialNode(0, nodeDir.UL);
            AddPotentialNode(1, nodeDir.R);
            AddPotentialNode(2, nodeDir.DL);
        }

        //finds a potential node from a given nodeID and direction
        public PotentialNode FindPotentialNode(int nodeID, nodeDir dir) {
            return FindPotentialNode(nodes[nodeID].coord + CoordFromNodeDir(dir));
        }

        //finds a potential node from a given node and direction
        public PotentialNode FindPotentialNode(Node n, nodeDir dir) {
            return FindPotentialNode(n.coord + CoordFromNodeDir(dir));
        }

        //finds a potential node at given coordinate
        public PotentialNode FindPotentialNode(Coord c) {
            foreach ( PotentialNode pn in potentialNodes ) {
                if ( pn.coord == c ) 
                    return pn;
            }
            Debug.LogError("FATAL ERROR: No Potential Nodes found at " + c.x.ToString() + ", " + c.y.ToString() + "!");
            return new PotentialNode();
        }

        //turns a potential node into a node found by ID
        public void MaterializePotentialNode(int nodeID, nodeDir dir) {
            MaterializePotentialNode(nodes[nodeID].coord + CoordFromNodeDir(dir));
        }

        //turns a potential node into a node
        public void MaterializePotentialNode(Node n, nodeDir dir) {
            MaterializePotentialNode(n.coord + CoordFromNodeDir(dir));
        }

        //turns a potential node into a node
        public void MaterializePotentialNode(Coord c) {
            PotentialNode pn = FindPotentialNode(c);
            potentialNodes.Remove(pn);

            Node newNode = new Node(nodeCount++);

            int fpn, lpn;
            fpn = lpn = -1;

            for ( int i = 0; i < 6; i++ ) {
                if ( pn.hasNeighbor[i] ) {
                    LinkNodes(ref pn.neighbors[i], ref newNode, Opposite((nodeDir)i) );
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

            //todo only update pnodes in proximity
            foreach ( PotentialNode allPn in potentialNodes ) {
                HexPotentialNode(allPn);
            }

            if ( fpn != -1 )
                AddPotentialNode(newNode, (nodeDir)fpn);
            if ( lpn != -1 )
                AddPotentialNode(newNode, (nodeDir)lpn);
        }

        //Links nodes
        public void LinkNodes(int n1id, int n2id, nodeDir n2_RelativeTo_n1) {
            Node n1 = nodes[n1id];
            Node n2 = nodes[n2id];
            LinkNodes(ref n1, ref n2, n2_RelativeTo_n1);
            nodes[n1id] = n1;
            nodes[n2id] = n2;
        }

        //Links nodes
        public void LinkNodes(ref Node n1, ref Node n2, nodeDir n2_RelativeTo_n1) {
            LinkNodes(ref n1, ref n2, n2_RelativeTo_n1, 0f);
        }

        //Links nodes, with weight
        public void LinkNodes(ref Node n1, ref Node n2, nodeDir n2_RelativeTo_n1, float weight) {
            n1.hasNeighbor[(int)n2_RelativeTo_n1] = true;
            n1.neighbors[(int)n2_RelativeTo_n1] = n2;
            n1.neighborCosts[(int)n2_RelativeTo_n1] = weight;

            nodeDir opposite = Opposite(n2_RelativeTo_n1);

            n2.coord = n1.coord + CoordFromNodeDir(n2_RelativeTo_n1);
            n2.hasNeighbor[(int)opposite] = true;
            n2.neighbors[(int)opposite] = n1;
            n2.neighborCosts[(int)opposite] = weight;
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

        //returns a or b randomly, weighted by percent values
        public int RandomWeighted(int a, int b, int percent0, int percent1) {
            int result = Random.Range(0, percent0 + percent1);
            return result>percent0 ? a : b;
        }

        //returns 0 or 1 randomly, weighted by percent values
        public int Random01Weighted(int percent0, int percent1) {
            int result = Random.Range(0, percent0 + percent1);
            return result>percent0 ? 1 : 0;
        }

        //returns an array of possible potential nodes to materialize next
        public Coord PickPotentialNode(pickMode mode) {
            List<PotentialNode> pnList = new List<PotentialNode>();
            int minCon = 7;
            int maxCon = 0;
            if ( mode != pickMode.Random ) {
                foreach (PotentialNode pn in potentialNodes) {
                    minCon = Mathf.Min(minCon, pn.GetNeighborCount());
                    maxCon = Mathf.Max(maxCon, pn.GetNeighborCount());
                }
            }

            //Debug.Log(minCon.ToString() + " " + maxCon.ToString());
            switch (mode) {
                case pickMode.Structural:
                    foreach ( PotentialNode pn in potentialNodes ) {
                        if ( pn.GetNeighborCount() == maxCon ) {
                            pnList.Add(pn);
                        }
                    }
                    break;
                case pickMode.SemiStructural:
                    foreach ( PotentialNode pn in potentialNodes ) {
                        if ( pn.GetNeighborCount() >= maxCon - Random01Weighted(60,40) ) {
                            pnList.Add(pn);
                        }
                    }
                    break;
                case pickMode.Random:
                    pnList = potentialNodes;
                    break;
                case pickMode.SemiChaotic:
                    foreach ( PotentialNode pn in potentialNodes ) {
                        if ( pn.GetNeighborCount() <= minCon + Random01Weighted(60,40) ) {
                            pnList.Add(pn);
                        }
                    }
                    break;
                case pickMode.Chaotic:
                    foreach ( PotentialNode pn in potentialNodes ) {
                        if ( pn.GetNeighborCount() == minCon ) {
                            pnList.Add(pn);
                        }
                    }
                    break;
            }
            return pnList[Random.Range(0, pnList.Count)].coord;
        }

        //Prints the graph to console with no pnodes by default
        public void DebugPrintGraph() {
            DebugPrintGraph(false);
        }

        //Prints the graph to console
        public void DebugPrintGraph(bool inclPotential) {
            
            ClearLog();

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
            nodeText += "| (" + pn.coord.x.ToString() + ", " + pn.coord.y.ToString() + ")";
            Debug.Log(nodeText);
        }

        //Prints all of a node's neighbors to console
        public void DebugPrintNeighbors(int nodeID) {
            Node n = nodes[nodeID];

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
            nodeText += "| (" + n.coord.x.ToString() + ", " + n.coord.y.ToString() + ")";
            Debug.Log(nodeText);
        }

        //clears editor console
        //thank you, anonymous stackoverflow user!
        public void ClearLog() {
            var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
        }

    }

}
