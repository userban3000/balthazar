using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour {
    
    [Header("Debug")]
    public int debugPlayerCount;

    [Header("Interaction")]
    public Player player;

    [Header("System")]
    StarSystem system;
    public bool DebugUseDummySystem;
    public StarSystem debugSystem;

    [Header("UI Elements")]
    public GameObject holder;
    public Text unitsShorthandText;
    public Text systemName;
    public Text owner;

    public void UpdatePD() {
        PlayerData.SetupPlayerData(debugPlayerCount);
    }

    public void EnableUI(StarSystem newSys) {
        holder.SetActive(true);
        TargetSystem(newSys);
    }

    public void DisableUI() {
        holder.SetActive(false);
    }

    private void Start() {

        //debug dummy sys
        if ( DebugUseDummySystem ) {
            TargetSystem(debugSystem);
        }

        holder.SetActive(false);
    }

    public void TargetSystem(StarSystem newSys) {
        system = newSys;
        systemName.text = system.systemName;
        owner.text = "Owned by " + PlayerData.GetPlayerName(system.teamIndex);
        owner.color = PlayerData.GetPlayerColor(system.teamIndex);
    }

    private void Update() {
        unitsShorthandText.text = Shorthand(system.units);

        if ( Input.GetMouseButton(1) || Input.GetKey(KeyCode.Escape) ) {
            DisableUI();
        }
    }

    public string Shorthand(int a) {
        string s = "";
        switch (a) {
            case var condition when ( a >= 0 && a < 10000 ):
                s = a.ToString();
                break;
            case var condition when ( a >= 10000 && a < 100000 ):
                s = (a / 1000).ToString() + "." + (a % 1000 / 100).ToString() + "k";
                break;
            case var condition when ( a >= 100000 && a < 1000000 ):
                s = (a / 1000).ToString() + "k";
                break;
            case var condition when ( a >= 1000000 ):
                s = (a / 1000000).ToString() + "M";
                break;
        }
        return s;
    }

}
