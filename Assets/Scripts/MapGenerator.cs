using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

using BalthazarGraph;

public enum MapSize {Zero, Tiny, Small, Medium, Large, Huge, Massive, Gargantuan, Ludicrous, PleaseDontPickThis};
public enum Connectivity {Webbed, Connected, Average, Displaced, Stranded};

public struct LineInfo {
    Coord from, to;

    public LineInfo (Coord f, Coord t) {
        from = f;
        to = t;
    }

    public static bool operator == (LineInfo l1, LineInfo l2) {
        return l1.from == l2.from && l1.to == l2.to;
    }

    public static bool operator != (LineInfo l1, LineInfo l2) {
        return !(l1==l2);
    }

    //code to get rid of warnings on not overriding Object.Equals and Object.GetHashCode
    public override bool Equals(object obj) {
            if ( obj is LineInfo l) {
                return this == l;
            }
            return false;
        }

        public override int GetHashCode() => new {from,to}.GetHashCode();
}

public class MapGenerator : MonoBehaviour {
    
    [Header("Seed")]
    public int seed;
    public bool randomSeed;

    [Header("Generation Settings")]
    public MapSize size;
    public Connectivity connectivity;
    public PickMode pick;

    [Header("Objects")]
    public GameObject systemHolder;
    public GameObject systemPrefab;
    public GameObject lineHolder;
    public GameObject linePrefab;

    [Header("Debug Settings")]
    public bool useCustomAmountOverPresetSizes;
    public int nodesToAdd;

    [Header("Internals")]
    static readonly int[] sizeNodeCounts = {0, 12, 30, 48, 102, 164, 224, 428, 724, 1892};
    static readonly float[] connectivityEdgeDeletionsCoefficients = {0.24f, 0.68f, 1f, 1.34f, 1.78f};
    static readonly float[] connectivityNodeDeletionsCoefficients = {0f, 0.66f, 1f, 1.86f, 3.33f};
    Graph g;

    [Header("Display")]
    static readonly string[] sizeNames = MapSize.GetNames(typeof(MapSize));
    static readonly string[] conNames = Connectivity.GetNames(typeof(Connectivity));

    public void GenerateMap() {
        MG_SetupNodes();
        MG_LoseEdges();
        MG_LoseNodes();
        MG_ConvertToWorldMap();
    }

    public void MG_SetupNodes() {
        //SEEDING
        if ( randomSeed ) 
            seed = Random.Range(-100000,100000);
        g = new Graph (seed);
        Debug.Log("Generating Graph with Seed: " + seed.ToString() + ", Size: " + sizeNames[(int)size] + ", Connectivity: " + conNames[(int)connectivity] + ".");

        //FIRST TRIANGLE
        g.GenerateFirstTriangle();

        //GET NODES TO ADD COUNT
        if ( !useCustomAmountOverPresetSizes ) {
            nodesToAdd = sizeNodeCounts[(int)size];
        }

        //ADD NODES
        for ( int i = 0; i < nodesToAdd; i++ ) {
            g.MaterializePotentialNode(g.PickPotentialNode(pick));
        }
    }

    public void MG_LoseEdges() {
        int edgesToDelete = (int)(sizeNodeCounts[(int)size] * 0.5f * connectivityEdgeDeletionsCoefficients[(int)connectivity]);
        Debug.Log("Deleting " + edgesToDelete.ToString() + " edges...");
        for ( int i = 0; i < edgesToDelete; i++ ) {
            int a = Random.Range(0, g.nodes.Count);
            int b;
            do {
                b = Random.Range(0, 6);
            } while ( !g.nodes[a].hasNeighbor[b] );
            g.DeleteEdge(g.nodes[a], (NodeDir)b);
        }
    }

    int debugStep = -1;
    public void MG_DebugLoseOneEdge() {
        debugStep++;
        /*
        do {
            b = Random.Range(0, 6);
        } while ( !g.nodes[a].hasNeighbor[b] );
        */

        g.DeleteEdge(g.nodes[g.NodeFromCoord(new Coord(0,0))], (NodeDir)debugStep );
        
        if ( debugStep == 6 )
            debugStep = -1;

        MG_Debug_ClearWorld();
        MG_ConvertToWorldMap();
    }

    public void MG_LoseNodes() {
        int nodesToDelete = (int)(sizeNodeCounts[(int)size] * 0.1f * connectivityNodeDeletionsCoefficients[(int)connectivity]);
        Debug.Log("Deleting " + nodesToDelete.ToString() + " nodes...");
        for ( int i = 0; i < nodesToDelete; i++ ) {
            int a = Random.Range(0, g.nodes.Count);
            g.DeleteNode(g.nodes[a]);
        }
    }

    public void MG_ConvertToWorldMap() {
        MG_Debug_ClearWorld();

        List<LineInfo> drawnLines = new List<LineInfo>();

        foreach ( Node n in g.nodes ) {
            Vector3 vN = new Vector3 (10*n.coord.x, -100, 20*n.coord.y);
            GameObject star = Instantiate(systemPrefab, vN, Quaternion.identity) as GameObject;
            star.name = "Star " + n.nodeID.ToString();
            star.transform.parent = systemHolder.transform;

            for ( int i = 0; i < 6; i++ ) {
                if ( n.hasNeighbor[i] ) {
                    LineInfo l = new LineInfo( n.coord, n.coord + g.CoordFromNodeDir((NodeDir)i) );
                    bool alreadyExists = false;
                    foreach ( LineInfo dl in drawnLines ) {
                        alreadyExists = dl == l;
                    }
                    if ( !alreadyExists ) {
                        MG_Connect(n.coord, n.coord + g.CoordFromNodeDir((NodeDir)i));
                    }
                }
            }
        }
    }

    public void MG_Connect(Coord c1, Coord c2) {
        Vector3[] pos = new Vector3[2];
        pos[0] = new Vector3 (10*c1.x, -100, 20*c1.y);
        pos[1] = new Vector3 (10*c2.x, -100, 20*c2.y);

        GameObject lineRendererGO = Instantiate (linePrefab, Vector3.zero, Quaternion.identity);
        LineRenderer lr = lineRendererGO.GetComponent<LineRenderer>();
        lr.transform.parent = lineHolder.transform;

        lr.SetPositions(pos);
    }

    public void MG_Debug_ClearGraph() {
        g = new Graph(0);
    }

    public void MG_Debug_ClearWorld() {
        GameObject[] systems = GameObject.FindGameObjectsWithTag("Star System");
        foreach ( GameObject go in systems ) {
            DestroyImmediate(go);
        }
        GameObject[] lines = GameObject.FindGameObjectsWithTag("Connection");
        foreach ( GameObject go in lines ) {
            DestroyImmediate(go);
        }
    }

    public void AddNode() {
        g.MaterializePotentialNode(g.PickPotentialNode(pick));
        Debug.Log("Added a node.");
    }

    public void ShowGraph() {
        g.DebugPrintGraph(true);
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
