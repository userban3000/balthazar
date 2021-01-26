using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendlyUI : MonoBehaviour {
    
    [Header("Debug")]
    public int debugPlayerCount;

    [Header("System")]
    StarSystem system;
    private int unitsToSend;
    public bool DebugUseDummySystem;
    public StarSystem debugSystem;

    [Header("UI Elements")]
    public Button send;
    public Toggle[] toggles;
    public InputField inField;
    public Image percentLock;
    public Text unitsText;
    public Text systemName;
    public Text owner;

    [Header("Size Presets")]
    private Vector3 toggleSizeOr;
    private Vector3 toggleSizeHi;
    private Vector3 toggleLabelSizeOr;
    private Vector3 toggleLabelSizeHi;
    private Vector3 offsetHi = new Vector3(0, 4.5f, 0);

    [Header("Sync")]
    public int syncVal;
    public bool sync;
    private bool percentLockVisible;
    private int tweenSyncID;
    private GameObject emptyGO;

    [Header("Input Field")]
    public Image iconLock;
    public Image iconUnlock;

    [Header("Colors")]
    public Color c_off;
    public Color c_on;
    public Color textDefault;
    public Color textUninteractable;

    public void UpdatePD() {
        PlayerData.SetupPlayerData(debugPlayerCount);
    }

    private void Start() {
        //get initial sizes
        toggleSizeOr = toggles[0].GetComponent<RectTransform>().localScale;
        toggleLabelSizeOr = toggles[0].GetComponentInChildren<Text>().GetComponent<RectTransform>().localScale;

        toggleSizeHi = new Vector3(toggleSizeOr.x, toggleSizeOr.y * 1.15f, toggleSizeOr.z);
        toggleLabelSizeHi = new Vector3(toggleSizeOr.x, toggleSizeOr.y * 0.87f, toggleSizeOr.z);

        //set percent lock popup
        GameObject plg = percentLock.gameObject;
        plg.transform.localScale = new Vector3 (1f, 0f, 1f);
        percentLockVisible = false;

        //set inputfield lockpad up
        iconLock.enabled = false;

        //debug dummy sys
        if ( DebugUseDummySystem ) {
            TargetSystem(debugSystem);
        }

        //empty go used for value tweening
        emptyGO = new GameObject();
    }

    public void TargetSystem(StarSystem newSys) {
        system = newSys;
        systemName.text = system.systemName;
        owner.text = "Owned by " + PlayerData.GetPlayerName(system.teamIndex);
        owner.color = PlayerData.GetPlayerColor(system.teamIndex);
    }

    private void Update() {
        unitsText.text = system.units.ToString();

        //constantly update inputfield text if sync is on
        if ( sync ) {
            if ( !LeanTween.isTweening(tweenSyncID) ) {
                unitsToSend = (int)(system.units * syncVal / 100);
            }
            inField.SetTextWithoutNotify(unitsToSend.ToString());
        }
    }

    public void PressedSend(){
        Debug.Log("Sent!");
    }

    public void ChangedToggle(int index) {
        Toggle t = toggles[index];
        RectTransform labelRect = t.GetComponent<RectTransform>();
        RectTransform textRect = t.GetComponentInChildren<Text>().GetComponent<RectTransform>();
        RectTransform imageRect = t.GetComponent<Image>().GetComponent<RectTransform>();

        if ( t.isOn ) {
            syncVal = (index + 1) * 25;
            float oldVal = unitsToSend;
            float desiredVal = (int)(system.units * syncVal / 100);
            tweenSyncID = LeanTween.value(emptyGO, oldVal, desiredVal, 0.4f).setOnUpdate( (float val) => {unitsToSend = (int)val;} ).setEaseOutQuint().id;
            LeanTween.move(t.gameObject, t.transform.position + offsetHi, 0.2f).setEaseOutQuint();
            LeanTween.scale(labelRect, toggleSizeHi, 0.2f).setEaseOutQuint();
            LeanTween.scale(textRect, toggleLabelSizeHi, 0.2f).setEaseOutQuint();
            LeanTween.color(imageRect, c_on, 0.2f).setEaseOutQuint();
        } else {
            LeanTween.move(t.gameObject, t.transform.position - offsetHi, 0.2f).setEaseOutQuint();
            LeanTween.scale(labelRect, toggleSizeOr, 0.2f).setEaseOutQuint();
            LeanTween.scale(textRect, toggleLabelSizeOr, 0.2f).setEaseOutQuint();
            LeanTween.color(imageRect, c_off, 0.2f).setEaseOutQuint();
        }

        sync = false;
        for ( int i = 0; i < 4; i++ ) {
            sync = sync || toggles[i].isOn;
        }

        if ( sync && !percentLockVisible) {
            SyncLock();
        } else if ( !sync && percentLockVisible ) {
            SyncUnlock();
        }

    }

    public void SyncLock() {
        GameObject plg = percentLock.gameObject;
        Vector3 plOffset = new Vector3(0, 20f, 0f);
        percentLockVisible = true;
        LeanTween.move(plg, plg.transform.position - plOffset, 0.4f).setEaseOutExpo();
        LeanTween.scaleY(plg, 1f, 0.4f).setEaseOutExpo();

        iconUnlock.enabled = false;
        iconLock.enabled = true;

        inField.textComponent.color = textUninteractable;
        inField.interactable = false;
    }

    public void SyncUnlock() {
        GameObject plg = percentLock.gameObject;
        Vector3 plOffset = new Vector3(0, 20f, 0f);
        percentLockVisible = false;
        LeanTween.move(plg, plg.transform.position + plOffset, 0.4f).setEaseOutExpo();
        LeanTween.scaleY(plg, 0f, 0.4f).setEaseOutExpo();

        iconUnlock.enabled = true;
        iconLock.enabled = false;

        inField.textComponent.color = textDefault;
        inField.interactable = true;
    }

}
