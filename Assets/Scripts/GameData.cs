using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class GameData {

    private static Dictionary<string, int> scienceUnlocks = new Dictionary<string, int>();

    private static int systemNameCount;
    private static string[] systemNames;
    private static bool[] systemNameUsed;

    public static Dictionary<string, int> GetAllScienceUnlocks() {
        return scienceUnlocks;
    }

    public static void SetScience() {
        Dictionary<string, int> sci = new Dictionary<string, int>();

        sci.Add("Subspace Warp Distortion", 1);

        scienceUnlocks = sci;
    }

    public static string GetRandomUnusedSystemName() {
        int a;
        do {
            a = Random.Range(0, systemNameCount);
        } while ( systemNameUsed[a] );
        systemNameUsed[a] = true;
        return systemNames[a];
    }

    public static void SetNames() {
        List<string> names = new List<string>();
        TextAsset sysNamesFromFile = Resources.Load<TextAsset>("systemNames");
        names.AddRange( sysNamesFromFile.text.Split( '\n' ) );

        systemNameCount = names.Count;

        systemNames = new string[systemNameCount];
        systemNames = names.ToArray();

        systemNameUsed = new bool[systemNameCount];
    }

    public static string GetRandomName() {
        int a;
        do {
            a = Random.Range(0, systemNameCount);
        } while ( systemNameUsed[a] );
        systemNameUsed[a] = true;
        return systemNames[a];
    }


}
