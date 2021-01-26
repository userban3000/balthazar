using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerData {

    private const int DBG_MAXPLAYERS = 8;  
    
    
    //PLAYER ORDER: 0 - EMPTY, 1-8 - PLAYERS
    public static string[] allPlayerHexColors = new string[] {
        "757575",
        "E53935",
        "D81B60",
        "8E24AA",
        "3949AB",
        "1E88E5",
        "00897B",
        "43A047",
        "7CB342",
        "FDD835",
        "FB8C00",
        "F4511E"
    };

    public static int playerCount;
    public static bool[,] friendlyTeams;
    //order is: unoccupied, p1, p2, p3... p8
    public static Color[] playerColors;
    public static string[] playerNames = new string[] {"neutral", "papa_alex", "theul", "deiu THE POG", "what if one of the names was just rly long" };

    private static void Start() {
        SetupPlayerData(1);
    }

    public static void SetupPlayerData(int players) {
        playerCount = players;
        playerColors = new Color[9];
        for ( int i = 0; i <= playerCount; i++ ) {
            playerColors[i] = ColorFromHex(allPlayerHexColors[i]);
        }
        friendlyTeams = new bool[DBG_MAXPLAYERS,DBG_MAXPLAYERS];
    }

    public static Color GetPlayerColor(int playerID) {
        return ColorFromHex(allPlayerHexColors[playerID]);
    }

    public static string GetPlayerName(int playerID) {
        return playerNames[playerID];
    }

    public static bool[] GetFriendlyTeams(int playerID) {
        bool[] playerFriendlyTeams = new bool[DBG_MAXPLAYERS];
        for ( int i = 0; i < DBG_MAXPLAYERS; i++ ) {
            playerFriendlyTeams[i] = friendlyTeams[playerID, i];
        }
        return playerFriendlyTeams;
    }

    public static Color ColorFromHex (string hex) {
        float r = FloatFromChar(hex[0]) * 16 + FloatFromChar(hex[1]);
        float g = FloatFromChar(hex[2]) * 16 + FloatFromChar(hex[3]);
        float b = FloatFromChar(hex[4]) * 16 + FloatFromChar(hex[5]);
        return new Color (r/255, g/255, b/255);
    }

    public static float FloatFromChar (char c) => c >= '0' && c <= '9' ? c - '0' : c - 'A' + 10; 

}
