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

    [Header("Control")]
    public float safeScreenPercent;

    [Header("UI")]
    public StarSystemUI starSysUIHolder;
    public Vector3 UI_Offset = new Vector3 (1f, 2f, 0f);

    private void Awake() {
        cam = FindObjectOfType<Camera>();
    }

    private void Update() {

        //SYSTEM SELECTION
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction*1000, Color.green);

        if ( Physics.Raycast(ray.origin, ray.direction*1000, out hit, Mathf.Infinity, starSystemLayer ) ) {
            GameObject modelGameObject = hit.collider.gameObject;
            hitSystem = modelGameObject.GetComponentInParent<StarSystem>() as StarSystem;
            isHovering = true;
        } else {
            isHovering = false;
        }

        Debug.Log(isHovering);

        if ( isHovering ) {
            starSysUIHolder.gameObject.SetActive(true);
            starSysUIHolder.Customize(hitSystem);
            starSysUIHolder.transform.position = Input.mousePosition + UI_Offset;
        } else {
            starSysUIHolder.gameObject.SetActive(false);
        }
        
    }

}
