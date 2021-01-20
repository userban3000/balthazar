using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//custom graph system for triangular diamond graphs
//based on US3K_Graph, which is also written by me
namespace BalthazarGraph {

//===================================================ENUMS===================================================

    //up right, right, down right, down left, left, up left
    public enum NodeDir {UR, R, DR, DL, L, UL};
    public enum PickMode {Crystal, Structured, Arranged, Indifferent, Shattered, Chaotic, Tentacular};

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
        public override bool Equals(object obj) {
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

        //get coord delta from a nodedir
        public Coord CoordFromNodeDir (NodeDir dir) {
            switch (dir) {
                case NodeDir.UR:
                    return Coord.UR;
                case NodeDir.R:
                    return Coord.R; 
                case NodeDir.DR:
                    return Coord.DR;
                case NodeDir.DL:
                    return Coord.DL;
                case NodeDir.L:
                    return Coord.L;
                case NodeDir.UL:
                    return Coord.UL;
                default:
                    return Coord.Zero;
            }
        }

        //opposite of direction
        public NodeDir Opposite(NodeDir direction) {
            return (int)direction < 3 ? ( direction + 3 ) : ( direction - 3 );
        }

        //Empty node constructor
        public Node ( int _nID ) {
            nodeID = _nID;
            coord = new Coord(0,0);
            hasNeighbor = new bool[6];
        }

        //Node constructor
        public Node ( int _nID, Coord _coord ) {
            nodeID = _nID;
            coord = _coord;
            hasNeighbor = new bool[6];
        }

        public int NeighborCount() {
            int nc = 0;
            for ( int i = 0; i < 6; i++ ) {
                if ( hasNeighbor[i] )
                    nc++;
            }
            return nc;
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
        public bool[] hasNeighbor;

        //get coord delta from a nodedir
        public Coord CoordFromNodeDir (NodeDir dir) {
            switch (dir) {
                case NodeDir.UR:
                    return Coord.UR;
                case NodeDir.R:
                    return Coord.R; 
                case NodeDir.DR:
                    return Coord.DR;
                case NodeDir.DL:
                    return Coord.DL;
                case NodeDir.L:
                    return Coord.L;
                case NodeDir.UL:
                    return Coord.UL;
                default:
                    return Coord.Zero;
            }
        }

        public PotentialNode ( int discard ) {
            coord = new Coord(0,0);
            hasNeighbor = new bool[6];
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
        public NodeDir Next(NodeDir direction) {
            return (int)direction < 5 ? ( direction + 1 ) : ( direction - 5 );
        }

        //prev direction required to run a full hexagon
        public NodeDir Prev(NodeDir direction) {
            return (int)direction < 1 ? ( direction + 5 ) : ( direction - 1 );
        }

        //converts an outward direction from an inner node to the direction it has to start following for traversing all of the nodes in its hex
        public NodeDir OutToSideways(NodeDir direction) {
            return (int)direction < 4 ? ( direction + 2 ) : ( direction - 4 );
        }

        //opposite of direction
        public NodeDir Opposite(NodeDir direction) {
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

        public Coord CoordFromNodeDir (NodeDir dir) {
            switch (dir) {
                case NodeDir.UR:
                    return Coord.UR;
                case NodeDir.R:
                    return Coord.R; 
                case NodeDir.DR:
                    return Coord.DR;
                case NodeDir.DL:
                    return Coord.DL;
                case NodeDir.L:
                    return Coord.L;
                case NodeDir.UL:
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
        public void AddNode(Node preconfigured) {
            Node n = new Node(0);
            n = preconfigured;
            n.nodeID = nodeCount++;
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
                    DeleteEdge(n, (NodeDir)i);
                }
            }
        }

        //fully removes node
        public void ShredNode(Node n) {
            nodes.Remove(n);
        }

        public int NodeFromCoord(Coord c) {
            int i;
            bool found = false;
            for ( i = 0; i < nodes.Count && !found; i++ ) {
                found = c == nodes[i].coord;
            }
            if ( !found )
                Debug.LogError("Looked for Node at (" + c.x.ToString() + "," + c.y.ToString() + "), but could not find it!");
            return i - 1;
        }

        public bool ExistsNodeFromCoord(Coord c) {
            bool found = false;
            for ( int i = 0; i < nodes.Count && !found; i++ ) {
                found = c == nodes[i].coord;
            }
            return found;
        }

        //delete edge between two nodes
        public void DeleteEdge(Node n1, NodeDir dir) {
            bool foundEdge = n1.hasNeighbor[(int)dir];

            if ( !foundEdge ) {
                Debug.LogError("Edge deletion with data NodeID: " + n1.nodeID.ToString() + ", Direction: " + ((int)dir).ToString() + " failed! Edge does not exist." );
                return;
            }
            
            n1.hasNeighbor[(int)dir] = false;
            if ( n1.NeighborCount() == 0 )
                ShredNode(n1);

            int n2 = NodeFromCoord(n1.coord + CoordFromNodeDir(dir));

            nodes[n2].hasNeighbor[(int)Opposite(dir)] = false;
            if ( nodes[n2].NeighborCount() == 0 )
                ShredNode(nodes[n2]);
        }

        //merges two conflicting potential nodes
        public PotentialNode MergePotentialNodes(PotentialNode mergeThis, PotentialNode intoThis) {
            PotentialNode pn = intoThis;
            for ( int i = 0; i < 6; i++ ) {
                if ( mergeThis.hasNeighbor[i] ) {
                    pn.hasNeighbor[i] = true;
                }
            }
            return pn;
        }

        //updates a potential node with its neighbors
        public void HexPotentialNode(ref PotentialNode pn) {
            for ( int i = 0; i < 6; i++ ) {
                if ( ExistsNodeFromCoord(pn.coord + CoordFromNodeDir((NodeDir)i) ) ) {
                    pn.hasNeighbor[i] = true;
                }
            }
        }

        //Adds a new potential node
        public void AddPotentialNode(Node n, NodeDir dir) {
            PotentialNode pn = new PotentialNode(0);
            pn.coord = n.coord + CoordFromNodeDir(dir);
            
            HexPotentialNode(ref pn);

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

        //generates the starting triangle of nodes
        public void GenerateFirstTriangle () {
            Node n0 = new Node(0, new Coord(0,0) );
            Node n1 = new Node(1, new Coord(1,1) );
            Node n2 = new Node(2, new Coord(2,0) );

            AddNode(n0); AddNode(n1); AddNode(n2);

            LinkNodes(0, 1, NodeDir.UR);
            LinkNodes(1, 2, NodeDir.DR);
            LinkNodes(0, 2, NodeDir.R);

            AddPotentialNode(nodes[0], NodeDir.UL);
            AddPotentialNode(nodes[1], NodeDir.R);
            AddPotentialNode(nodes[2], NodeDir.DL);
        }

        //finds a potential node at given coordinate
        public PotentialNode FindPotentialNode(Coord c) {
            foreach ( PotentialNode pn in potentialNodes ) {
                if ( pn.coord == c ) 
                    return pn;
            }
            Debug.LogError("No Potential Nodes found at " + c.x.ToString() + ", " + c.y.ToString() + "!");
            return new PotentialNode();
        }

        //turns a potential node into a node
        public void MaterializePotentialNode(Coord c) {
            PotentialNode pn = FindPotentialNode(c);
            
            potentialNodes.Remove(pn);
            Node n = new Node(0);
            n.coord = c;
            AddNode(n);

            int fpn, lpn;
            fpn = lpn = -1;

            for ( int i = 0; i < 6; i++ ) {
                if ( pn.hasNeighbor[i] ) {
                    LinkNodes(NodeFromCoord(c), NodeFromCoord(c + CoordFromNodeDir((NodeDir)i)), (NodeDir)i );
                    if ( !pn.hasNeighbor[(int)Next((NodeDir)i)] ) {
                        lpn = (int)Next((NodeDir)i);
                    }
                    if ( fpn == -1 ) {
                        if ( !pn.hasNeighbor[(int)Prev((NodeDir)i)] ) {
                            fpn = (int)Prev((NodeDir)i);
                        }
                    }
                }
            }

            //todo only update pnodes in proximity
            for ( int i = 0; i < potentialNodes.Count; i++ ) {
                PotentialNode updater = potentialNodes[i];
                HexPotentialNode(ref updater);
                potentialNodes[i] = updater;
            }

            if ( fpn != -1 )
                AddPotentialNode(nodes[nodes.Count-1], (NodeDir)fpn);
            if ( lpn != -1 )
                AddPotentialNode(nodes[nodes.Count-1], (NodeDir)lpn);
        }

        //connects nodes
        public void LinkNodes (int n1, int n2, NodeDir n2_RelativeTo_n1) {
            nodes[n1].hasNeighbor[(int)n2_RelativeTo_n1] = true;
            nodes[n2].hasNeighbor[(int)Opposite(n2_RelativeTo_n1)] = true;
        }

        //opposite of direction
        public NodeDir Opposite(NodeDir direction) {
            return (int)direction < 3 ? ( direction + 3 ) : ( direction - 3 );
        }

        //next direction required to run a full hexagon
        public NodeDir Next(NodeDir direction) {
            return (int)direction < 5 ? ( direction + 1 ) : ( direction - 5 );
        }

        //prev direction required to run a full hexagon backwards (ccw)
        public NodeDir Prev(NodeDir direction) {
            return (int)direction < 1 ? ( direction + 5 ) : ( direction - 1 );
        }

        //converts an outward direction from an inner node to the direction it has to start following for traversing all of the nodes in its hex
        public NodeDir OutToSideways(NodeDir direction) {
            return (int)direction < 4 ? ( direction + 2 ) : ( direction - 4 );
        }

        //returns a or b randomly, weighted by percent values
        public int RandomWeighted(int a, int b, int percent0, int percent1) {
            int result = Random.Range(0, percent0 + percent1);
            return result>percent0 ? a : b;
        }

        //returns 0 or 1 randomly, weighted by percent values
        public int Random01Weighted(int percent0, int percent1) {
            return RandomWeighted(0, 1, percent0, percent1);
        }

        //returns an array of possible potential nodes to materialize next
        public Coord PickPotentialNode(PickMode mode) {
            List<PotentialNode> pnList = new List<PotentialNode>();
            int minCon = 7;
            int maxCon = 0;
            if ( mode != PickMode.Indifferent ) {
                foreach (PotentialNode pn in potentialNodes) {
                    minCon = Mathf.Min(minCon, pn.GetNeighborCount());
                    maxCon = Mathf.Max(maxCon, pn.GetNeighborCount());
                }
            }

            switch (mode) {
                case PickMode.Crystal:
                    foreach ( PotentialNode pn in potentialNodes ) {
                        if ( pn.GetNeighborCount() == maxCon ) {
                            pnList.Add(pn);
                        }
                    }
                    break;
                case PickMode.Structured:
                    foreach ( PotentialNode pn in potentialNodes ) {
                        if ( pn.GetNeighborCount() >= maxCon - Random01Weighted(70,30) ) {
                            pnList.Add(pn);
                        }
                    }
                    break;
                case PickMode.Arranged:
                    foreach ( PotentialNode pn in potentialNodes ) {
                        if ( pn.GetNeighborCount() >= maxCon - 1 - Random01Weighted(30,70) ) {
                            pnList.Add(pn);
                        }
                    }
                    break;
                case PickMode.Indifferent:
                    pnList = potentialNodes;
                    break;
                case PickMode.Shattered:
                    foreach ( PotentialNode pn in potentialNodes ) {
                        if ( pn.GetNeighborCount() <= minCon + Random01Weighted(70,30) ) {
                            pnList.Add(pn);
                        }
                    }
                    break;
                case PickMode.Chaotic:
                    foreach ( PotentialNode pn in potentialNodes ) {
                        if ( pn.GetNeighborCount() <= minCon + 1 + Random01Weighted(20,80) ) {
                            pnList.Add(pn);
                        }
                    }
                    break;
                case PickMode.Tentacular:
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

        //returns a node's list position
        public int PosInList(Node n) {
            int i;
            bool found = false;
            for ( i = 0; i < nodes.Count && !found; i++ ) {
                if ( n == nodes[i] )
                    found = true;
            }
            if ( !found )
                Debug.LogError("Searched for list index of Node at Coord (" + n.coord.x + "," + n.coord.y + "), but could not find it.");
            return i - 1;
        }

        //depth first tagging
        public void DFS(Node start, ref bool[] visited) {
            List<Node> q = new List<Node>();
            q.Add(start);

            int index = 0;

            while ( index < q.Count ) {
                Node n = q[index];

                visited[PosInList(n)] = true;
                for ( int i = 0; i < 6; i++ ) {
                    if ( n.hasNeighbor[i] && !visited[NodeFromCoord(n.coord + CoordFromNodeDir((NodeDir)i))] ) {
                        q.Add(nodes[NodeFromCoord(n.coord + CoordFromNodeDir((NodeDir)i))]);
                    }
                }

                index++;
            }
        }

        public int ConstellationCount() {
            return ConstellationCount(-1);
        }

        //Returns number of constellations
        public int ConstellationCount(int preMarkNodePos) {
            int count = 0;
            bool[] visited = new bool[nodes.Count];

            if ( preMarkNodePos != -1 ) {
                visited[preMarkNodePos] = true;
            }

            foreach ( Node n in nodes ) {
                if ( !visited[PosInList(n)] ) {
                    DFS(n, ref visited);
                    count++;
                }
            }
            return count;
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

            string[] dirNames = NodeDir.GetNames(typeof(NodeDir));

            string nodeText = "Potential Node | ";
            for ( int i = 0; i < 6; i++ ) {
                if ( pn.hasNeighbor[i] ) {
                    nodeText += dirNames[i] + ": " + NodeFromCoord(pn.coord + CoordFromNodeDir((NodeDir)i)).ToString() + ", ";
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
            string[] dirNames = NodeDir.GetNames(typeof(NodeDir));

            string nodeText = "Node: " + n.nodeID.ToString() + " | ";
            for ( int i = 0; i < 6; i++ ) {
                if ( n.hasNeighbor[i] ) {
                    nodeText += dirNames[i] + ": " + NodeFromCoord(n.coord + CoordFromNodeDir((NodeDir)i)).ToString() + ", ";
                }
            }
            nodeText += "| (" + n.coord.x.ToString() + ", " + n.coord.y.ToString() + ")";
            Debug.Log(nodeText);
        }

    }

}
