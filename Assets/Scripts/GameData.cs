using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class GameData {

    private static Dictionary<string, int> scienceUnlocks = new Dictionary<string, int>();

    private static int systemNameCount;
    public static string[] systemNames;

    public static Dictionary<string, int> GetAllScienceUnlocks() {
        return scienceUnlocks;
    }

    public static void SetScience() {
        Dictionary<string, int> sci = new Dictionary<string, int>();

        sci.Add("Subspace Warp Distortion", 1);

        scienceUnlocks = sci;
    }

    //fisher-yates
    public static void ShuffleNames() {
        int n = systemNames.Length;

        for ( int i = n - 1; i > 0; i-- ) {
            int j = Random.Range(0, i + 1);
            string swapper = systemNames[i];
            systemNames[i] = systemNames[j];
            systemNames[j] = swapper;
        }
    }

    public static void SetNames() {
        List<string> names = new List<string>();
        TextAsset sysNamesFromFile = Resources.Load<TextAsset>("systemNames");
        names.AddRange( sysNamesFromFile.text.Split( '\n' ) );

        systemNameCount = names.Count;

        systemNames = new string[systemNameCount];
        systemNames = names.ToArray();

        ShuffleNames();
    }

}
