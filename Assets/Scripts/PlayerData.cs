using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerData {

    private const int DBG_MAXPLAYERS = 8;  
    
    /*
    public int playerID;
    public bool[] scienceUnlocks;
    public bool[] friendlyTeams;

    public Dictionary<string, int> sciNameToID;

    public PlayerData (int _pid) {
        playerID = _pid;
        scienceUnlocks = new bool[100]; //to be changed into a more sensible value later
        sciNameToID = GameData.GetAllScienceUnlocks();
        friendlyTeams = new bool[8];
        friendlyTeams[playerID] = true;
    }
    */

    public static int playerCount;
    public static bool[,] friendlyTeams;
    //order is: unoccupied, p1, p2, p3... p8
    public static Color[] playerColors = new Color[] {new Color(0.2f, 0.2f, 0.2f, 1f), Color.red, Color.green, Color.blue, Color.yellow};

    private static void Start() {
        SetupPlayerData();
    }

    public static void SetupPlayerData() {
        playerCount = DBG_MAXPLAYERS;
        playerColors = new Color[] {Color.grey, Color.red, Color.green, Color.blue, Color.yellow};
        friendlyTeams = new bool[DBG_MAXPLAYERS,DBG_MAXPLAYERS];
    }

    public static Color GetPlayerColor(int playerID) {
        return playerColors[playerID];
    }

    public static bool[] GetFriendlyTeams(int playerID) {
        bool[] playerFriendlyTeams = new bool[DBG_MAXPLAYERS];
        for ( int i = 0; i < DBG_MAXPLAYERS; i++ ) {
            playerFriendlyTeams[i] = friendlyTeams[playerID, i];
        }
        return playerFriendlyTeams;
    }

}
