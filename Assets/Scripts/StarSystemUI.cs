﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarSystemUI : MonoBehaviour {
    
    [Header("Data")]
    public StarSystem sys;

    [Header("UI")]
    public Canvas UI_Canvas;
    public Text UI_Units;
    public Text UI_UnitGenStats;
    public Text UI_SystemName;
    public Button UI_Send;
    public InputField UI_InputField;
    public int UTS_Link = -1;

    private void Awake() {
        UI_Canvas.enabled = false;
    }

    public void OpenUI() {
        UI_Canvas.enabled = true;
        UI_SystemName.text = sys.systemName;
    }

    public void CloseUI() {
        UI_Canvas.enabled = false;
        UTS_Link = -1;
    }

    public void Update() {
        if ( UI_Canvas.enabled ) {
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
            Debug.Log("Sent an army " + sys.unitsToSend.ToString() + " strong! The opponents fear us...");
            sys.units -= sys.unitsToSend;
        }
    }
}