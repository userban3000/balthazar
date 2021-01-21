using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour {

    [Header("Interaction Settings")]
    public LayerMask starSystemLayer;

    [Header("Internal Variables")]
    private Camera cam;
    private Vector3 dragOrigin;

    [Header("System Info")]
    StarSystem hitSystem;
    bool isHovering;

    [Header("UI")]
    public StarSystemUI SSUI;

    Vector3 mPos;

    private void Awake() {
        cam = GetComponent<Camera>();
    }

    private void Update() {

        //SYSTEM SELECTION
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        mPos = Input.mousePosition;

        Debug.DrawRay(ray.origin, ray.direction*1000, Color.green);

        if ( Physics.Raycast(ray.origin, ray.direction*1000, out hit, Mathf.Infinity, starSystemLayer ) && PointerIsOnScreen()) {
            GameObject modelGameObject = hit.collider.gameObject;
            hitSystem = modelGameObject.GetComponentInParent<StarSystem>() as StarSystem;
            isHovering = true;
        } else {
            isHovering = false;
        }

        if ( isHovering ) {
            hitSystem.particle.Play();
            if ( Input.GetMouseButtonDown(0) ) {
                SSUI.sys = hitSystem;
                SSUI.OpenUI();
            }
        } else if ( hitSystem != null ) {
            hitSystem.particle.Clear();
            hitSystem.particle.Stop();
        }
        
    }

    private bool PointerIsOnScreen() {
        return mPos.y <= Screen.height && mPos.y >= 0 && mPos.x <= Screen.width && mPos.x >= 0; 
    }

}
