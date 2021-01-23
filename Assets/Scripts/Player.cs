using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [Header("Presets")]
    public Camera cam;
    public GameObject armyPrefab;

    [Header("UI")]
    public StarSystemUI SSUI;
    public LineRenderer linePrefab;
    private LineRenderer lr;

    [Header("Mouse & Input Settings")]
    public float dragSpeed;
    public float scrollSpeed;
    [Range(0.001f,10f)]
    public float zoomSmoothing;
    Vector3 mPos;

    [Header("Camera Movement Values")]
    private float yVal;
    private float yZoom;
    private readonly float maxZoom = -90f;
    private readonly float minZoom = 260f;

    [Header("Interaction")]
    public LayerMask starSystemLayer;
    [Range(0.1f, 20f)]
    public float selectionSmoothing;
    private static readonly Plane gamePlane = new Plane(Vector3.up, new Vector3(0,-100,0));
    private Vector3 dragOrigin;
    private Vector3 OnPlane;
    private Vector3 lrSelection;
    public bool isChoosingDirection = false;
    public bool isHovering;
    private StarSystem hitSystem;

    [Header("Miscellaneous")]
    private bool hexDirections = false;

    private void Update() {

        Vector3 pos = cam.transform.position;

        //=====================================================================================DRAGGING
        if (Input.GetMouseButton(0)) {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
            pos.x -= Input.GetAxisRaw("Mouse X") * dragSpeed * ( DragSpeedAmp(yVal) ) * Time.deltaTime;
            pos.z -= Input.GetAxisRaw("Mouse Y") * dragSpeed * ( DragSpeedAmp(yVal) ) * Time.deltaTime;
        } else {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        //================================================================================ZOOMING
        if ( !SSUI.UI_SystemHolder.activeSelf )
            yVal -= Input.mouseScrollDelta.y * scrollSpeed;
        yVal = Mathf.Clamp(yVal, maxZoom, minZoom);

        //================================================================================SMOOTH THE ZOOMING
        StartCoroutine(SmoothZoom());

        //================================================================================SYSTEM SELECTION VARS
        Ray camRay = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        mPos = Input.mousePosition;
        float dist;

        //================================================================================SYSTEM SELECT DETECTION
        if ( Physics.Raycast(camRay.origin, camRay.direction, out hit, Mathf.Infinity, starSystemLayer ) && PointerIsOnScreen() && !SSUI.UI_SystemHolder.activeSelf && !isChoosingDirection ) {
            GameObject modelGameObject = hit.collider.gameObject;
            hitSystem = modelGameObject.GetComponentInParent<StarSystem>() as StarSystem;
            isHovering = true;
        } else {
            isHovering = false;
        }

        //================================================================================SYSTEM SELECT ANIMATION && CAPTURING MOUSE CLICK FOR UI OPEN
        if ( isHovering || SSUI.UI_SystemHolder.activeSelf) {
            hitSystem.particle.Play();
            if ( Input.GetMouseButtonDown(0) ) {
                SSUI.sys = hitSystem;
                SSUI.OpenUI();
            }
        } else if ( hitSystem != null && !isChoosingDirection ) {
            hitSystem.particle.Clear();
            hitSystem.particle.Stop();
        }

        //================================================================================CHOOSING DIR, AFTER SELECTING TO SEND UNITS
        if ( isChoosingDirection ) {
            SSUI.UI_SystemSelectionTooltip.enabled = true;

            if ( gamePlane.Raycast(camRay, out dist)) {
                OnPlane = camRay.GetPoint(dist);
            }

            Ray targetRay = new Ray(hitSystem.transform.position, OnPlane - hitSystem.transform.position);
            Debug.DrawRay(targetRay.origin, targetRay.direction*20000, Color.green);
            RaycastHit fromPlanetHit;
            RaycastHit fromCamHit;
            StarSystem targetedSys = null;
            //this massive if says "if we hover and theres a path to said system" basically
            if ( Physics.Raycast(targetRay.origin, targetRay.direction, out fromPlanetHit, 2000f, starSystemLayer) && Physics.Raycast(camRay.origin, camRay.direction, out fromCamHit, Mathf.Infinity, starSystemLayer ) && fromPlanetHit.collider == fromCamHit.collider ) {
                targetedSys = fromPlanetHit.collider.gameObject.GetComponentInParent<StarSystem>() as StarSystem;        
            }    

            if ( hexDirections ) {
                
            } else {
                lr.SetPosition(0, hitSystem.transform.position);
                if ( targetedSys != null ) {
                    StartCoroutine(SmoothSelection(targetedSys.transform.position));
                }
                lr.SetPosition(1, lrSelection);
            }

            //confirming
            if ( Input.GetMouseButton(0) && targetedSys != null ) {
                GameObject armyGO = Instantiate(armyPrefab, hitSystem.transform.position + (targetedSys.transform.position - hitSystem.transform.position).normalized * 3.2f, Quaternion.LookRotation(targetedSys.transform.position - hitSystem.transform.position));
                Army army = armyGO.GetComponent<Army>();

                army.Set(hitSystem.unitsToSend, new PlayerData(0) );

                hitSystem.particle.Clear();
                hitSystem.particle.Stop();

                isChoosingDirection = false;
                SSUI.UI_SystemHolder.SetActive(false);
                SSUI.UI_SystemSelectionTooltip.enabled = false;
                Destroy(lr);
                return;
            }

            //cancel
            if ( Input.GetMouseButton(1) ) {
                isChoosingDirection = false;
                SSUI.UI_SystemHolder.SetActive(false);
                SSUI.UI_SystemSelectionTooltip.enabled = false;
                Destroy(lr);
                return;
            }
        }

        //================================================================================CAMERA MOVEMENT
        if ( !SSUI.UI_SystemHolder.activeSelf )
            cam.transform.position = new Vector3(pos.x, yZoom, pos.z);
        
    }

    //Handles choosing directions
    public void ChooseDir() {
        isChoosingDirection = true;
        lr = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        lrSelection = hitSystem.transform.position;
        lr.SetPosition(1, lrSelection);
    }

    public Vector3 CrushToCoordSpace() {
        return Vector3.zero;
    }
    
    //returns true when pointer is inside windows
    private bool PointerIsOnScreen() {
        return mPos.y <= Screen.height && mPos.y >= 0 && mPos.x <= Screen.width && mPos.x >= 0; 
    }

    //remaps (-90, 260) to (0.8f, 1.5f)
    private float DragSpeedAmp(float a) {
        return (a + 90) / 200 + 0.1f;
    }

    //smoothes between zoom values
    IEnumerator SmoothZoom() {
        float t = 0;
        float percent = 0;

        while ( percent < 1 ) {
            t += Time.deltaTime;
            percent = t * zoomSmoothing;
            yZoom = Mathf.Lerp(cam.transform.position.y, yVal, percent);
            yield return null;
        }
    }

    IEnumerator SmoothSelection(Vector3 to) {
        float t = 0;
        float percent = 0;
        Vector3 lrOld = lr.GetPosition(1);

        while ( percent < 1 ) {
            t += Time.deltaTime;
            percent = ( -t*t + 2*t ) * selectionSmoothing; //-x^2 + 2x
            lrSelection = Vector3.Lerp(lrOld, to, percent);
            yield return null;
        }
    }
}
