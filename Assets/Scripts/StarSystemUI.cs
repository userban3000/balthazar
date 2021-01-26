using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarSystemUI : MonoBehaviour {
    
    [Header("Data")]
    public StarSystem sys;

    [Header("Friendly UI")]
    public GameObject UI_SystemHolder;
    public Text UI_SystemSelectionTooltip;
    public Text UI_SystemPreSelectionTooltip;
    public Text UI_SystemObstructionTooltip;
    public Text UI_Units;
    public Text UI_UnitGenStats;
    public Text UI_SystemName;
    public Button UI_Send;
    public InputField UI_InputField;
    public int UTS_Link = -1;

    [Header("Enemy UI")]
    public GameObject EUI_SystemHolder;
    public Text EUI_SystemName;
    public Text EUI_SystemUnits;

    [Header("Team-Based Coloring")]
    public Image UI_namePanel;
    public Image EUI_namePanel;

    private void Awake() {
        UI_SystemHolder.SetActive(false);

        UI_SystemSelectionTooltip.enabled = false;
        UI_SystemPreSelectionTooltip.enabled = false;
        UI_SystemObstructionTooltip.enabled = false;

        EUI_SystemHolder.SetActive(false);
    }

    public void OpenEUI() {

    }

    public void OpenUI() {
        UI_SystemHolder.SetActive(true);
        UI_SystemName.text = sys.systemName;
        UI_InputField.textComponent.text = "0";
        UI_Send.interactable = false;
        
        UI_namePanel.color = PlayerData.GetPlayerColor(sys.teamIndex);
    }

    public void CloseUI() {
        UI_SystemHolder.SetActive(false);
        UTS_Link = -1;
    }

    public void Update() {
        if ( UI_SystemHolder.activeSelf ) {
            UI_Units.text = sys.units.ToString();
            UI_UnitGenStats.text = "(" + (sys.industryPerTick * 50).ToString() + " Ind/Sec, " + sys.industryToGenerateUnit.ToString() + " Ind/Unit)";
            
            if ( UTS_Link >= 0 ) {
                sys.unitsToSend = (int)(sys.units * UTS_Link / 100);
                UI_InputField.SetTextWithoutNotify(sys.unitsToSend.ToString());
            }

            if ( sys.unitsToSend > sys.units ) {
                UI_Send.interactable = false;
            } else {
                UI_Send.interactable = true;
            }

            if ( Input.GetMouseButtonDown(1) ) {
                CloseUI();
            }
        } 
    }

    public void StopLinkingUnitsToSend() {
        UTS_Link = -1;
        UI_InputField.textComponent.text = null;
    }

    public void LinkUnitsToSend(int percent) {
        UTS_Link = percent;
    }

    public void InputFieldUnitsToSend() {
        string s = UI_InputField.textComponent.text;
        
        UTS_Link = -1;
        sys.unitsToSend = int.Parse(s);
        if ( sys.unitsToSend > sys.units ) {
                UI_Send.interactable = false;
            } else {
                UI_Send.interactable = true;
            }
    }

    public void SendArmy() {
        if ( UI_Send.interactable ) {
            //Debug.Log("Sent an army " + sys.unitsToSend.ToString() + " strong! The opponents fear us...");
            sys.units -= sys.unitsToSend;

            Player player = FindObjectOfType<Player>();
            CloseUI();
            player.ChooseDir();
        }
    }
}