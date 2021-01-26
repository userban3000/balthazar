using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UI_Toggle : MonoBehaviour {
    
    public Toggle toggle;

    private void Start() {
        //toggle.OnPointerEnter
    }

    private void OnMouseEnter() {
        Debug.Log("hover");
    }

    private void OnMouseExit() {
        Debug.Log("stopped hovering");
    }

}
