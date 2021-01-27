using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandlerUI : MonoBehaviour {
    
    public Player player;
    public FriendlyUI FUI;
    public EnemyUI EUI;

    public void Focus(StarSystem sys) {
        FUI.DisableUI();
        EUI.DisableUI();

        if ( player.playerID == sys.teamIndex ) {
            FUI.EnableUI(sys);
        } else {
            EUI.EnableUI(sys);
        }
    }

    //are any UI windows currently active?
    public bool IsAnyWindowActive() {
        return FUI.isActiveAndEnabled;
    }

    public void DisableCurrentlyActiveUI() {
        FUI.DisableUI();
    }
}
