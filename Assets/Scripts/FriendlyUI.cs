using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendlyUI : MonoBehaviour {
    
    [Header("Debug")]
    public int debugPlayerCount;

    [Header("Interaction")]
    public Player player;

    [Header("System")]
    public StarSystem system;
    public int unitsToSend;
    public bool DebugUseDummySystem;
    public StarSystem debugSystem;

    [Header("UI Elements")]
    public GameObject holder;
    public Button send;
    public Toggle[] toggles;
    public InputField inField;
    public Image percentLock;
    public Text unitsText;
    public Text unitsShorthandText;
    public Text systemName;
    public Text owner;

    [Header("Movement Vector3's")]
    private Vector3 toggleSizeOr;
    private Vector3 toggleSizeHi;
    private Vector3 toggleLabelSizeOr;
    private Vector3 toggleLabelSizeHi;
    private Vector3[] toggleOriginalLoc = new Vector3[4];
    private Vector3[] toggleHiLoc = new Vector3[4];
    private Vector3 plgOn;
    private Vector3 plgOff;
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
    public Color c_textDefault;
    public Color c_textUninteractable;

    public void UpdatePD() {
        PlayerData.SetupPlayerData(debugPlayerCount);
    }

    public void EnableUI(StarSystem newSys) {
        holder.SetActive(true);
        TargetSystem(newSys);
    }

    public void DisableUI() {
        holder.SetActive(false);
        sync = false;
        inField.textComponent.text = "";
    }

    private void Start() {
        //get initial label parameters
        toggleSizeOr = toggles[0].GetComponent<RectTransform>().localScale;
        toggleLabelSizeOr = toggles[0].GetComponentInChildren<Text>().GetComponent<RectTransform>().localScale;

        toggleSizeHi = new Vector3(toggleSizeOr.x, toggleSizeOr.y * 1.15f, toggleSizeOr.z);
        toggleLabelSizeHi = new Vector3(toggleSizeOr.x, toggleSizeOr.y * 0.87f, toggleSizeOr.z);

        for ( int i = 0; i < 4; i++ ) {
            toggleOriginalLoc[i] = toggles[i].transform.position;
            toggleHiLoc[i] = toggleOriginalLoc[i] + new Vector3(0, 4.5f, 0);;
        }

        //set percent lock popup
        GameObject plg = percentLock.gameObject;
        plg.transform.localScale = new Vector3 (1f, 0f, 1f);
        percentLockVisible = false;
        plgOn = plg.transform.position - new Vector3(0, 20f, 0f);
        plgOff = plg.transform.position;

        //set inputfield lockpad up
        iconLock.enabled = false;

        //debug dummy sys
        if ( DebugUseDummySystem ) {
            TargetSystem(debugSystem);
        }

        //empty go used for value tweening
        emptyGO = new GameObject();

        holder.SetActive(false);
    }

    public void TargetSystem(StarSystem newSys) {
        system = newSys;
        systemName.text = system.systemName;
        owner.text = "Owned by " + PlayerData.GetPlayerName(system.teamIndex);
        owner.color = PlayerData.GetPlayerColor(system.teamIndex);
    }

    private void Update() {
        unitsText.text = system.units.ToString();
        //unitsShorthandText.text = Shorthand(system.units);

        //constantly update inputfield text if sync is on
        if ( sync ) {
            if ( !LeanTween.isTweening(tweenSyncID) ) {
                unitsToSend = (int)(system.units * syncVal / 100);
            }
            if ( send.interactable == false ) {
                send.interactable = true;
                send.GetComponentInChildren<Text>().color = c_textDefault;
            }
            inField.SetTextWithoutNotify(unitsToSend.ToString());
        } else { //check if we want too many units
            if ( unitsToSend > system.units ) {
                send.interactable = false;
                send.GetComponentInChildren<Text>().color = c_textUninteractable;
            } else {
                send.interactable = true;
                send.GetComponentInChildren<Text>().color = c_textDefault;
            }
        }

        if ( Input.GetMouseButton(1) || Input.GetKey(KeyCode.Escape) ) {
            DisableUI();
        }
    }

    public void UpdateFromInputField() {
        Invoke("DelayedUpdateFromInputField", 0.1f);
    }

    public void DelayedUpdateFromInputField() {
        if ( inField.textComponent.text != "" ) {
            unitsToSend = System.Int32.Parse(inField.textComponent.text);
        }
    }

    public void PressedSend(){
        system.unitsToSend = unitsToSend;
        system.units -= unitsToSend;
            
        player.ChooseDir();
        DisableUI();
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
            LeanTween.move(t.gameObject, toggleHiLoc[index], 0.2f).setEaseOutQuint();
            LeanTween.scale(labelRect, toggleSizeHi, 0.2f).setEaseOutQuint();
            LeanTween.scale(textRect, toggleLabelSizeHi, 0.2f).setEaseOutQuint();
            LeanTween.color(imageRect, c_on, 0.2f).setEaseOutQuint();
        } else {
            LeanTween.move(t.gameObject, toggleOriginalLoc[index], 0.2f).setEaseOutQuint();
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

    public void SyncLock() {
        GameObject plg = percentLock.gameObject;
        percentLockVisible = true;
        LeanTween.move(plg, plgOn, 0.4f).setEaseOutExpo();
        LeanTween.scaleY(plg, 1f, 0.4f).setEaseOutExpo();

        iconUnlock.enabled = false;
        iconLock.enabled = true;

        inField.textComponent.color = c_textUninteractable;
        inField.interactable = false;
    }

    public void SyncUnlock() {
        GameObject plg = percentLock.gameObject;
        Vector3 plOffset = new Vector3(0, 20f, 0f);
        percentLockVisible = false;
        LeanTween.move(plg, plgOff, 0.4f).setEaseOutExpo();
        LeanTween.scaleY(plg, 0f, 0.4f).setEaseOutExpo();

        iconUnlock.enabled = true;
        iconLock.enabled = false;

        inField.textComponent.color = c_textDefault;
        inField.interactable = true;
    }

}
