using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData {
    
    public int playerID;
    public bool[] scienceUnlocks;

    public Dictionary<string, int> sciNameToID;

    public PlayerData (int _pid) {
        playerID = _pid;
        scienceUnlocks = new bool[100]; //to be changed into a more sensible value later
        sciNameToID = GameData.GetAllScienceUnlocks();
    }

}
