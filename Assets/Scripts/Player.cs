using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [Header("Multiplayer")]
    public int playerID;

    [Header("Presets")]
    public GameObject camHolder;
    public Camera cam;
    public GameObject armyPrefab;

    [Header("UI")]
    public StarSystemUI SSUI;

    [Header("Mouse & Input Settings")]
    public float dragSpeed;
    public float scrollSpeed;
    [Range(0.001f,10f)]
    public float zoomSmoothing;
    Vector3 mPos;

    [Header("Camera Movement Values")]
    public float yWish;
    public float yReal;
    public float tiltWish;
    public float tiltReal;
    public float offsetWish;
    public float offsetReal;
    private readonly float maxZoom = -90f;
    private readonly float minZoom = 260f;

    [Header("Interaction")]
    public LayerMask starSystemLayer;
    public LayerMask starSystemHoverLayer;
    [Range(0.1f, 20f)]
    public float selectionSmoothing;
    private static readonly Plane gamePlane = new Plane(Vector3.up, new Vector3(0,-100,0));
    private Vector3 dragOrigin;
    private Vector3 OnPlane;
    public bool isChoosingDirection = false;
    public bool isHovering;
    private StarSystem hoveredSystem;
    private StarSystem targetedSys;

    [Header("Line Renderer")]
    public LineRenderer linePrefab;
    private LineRenderer lr;
    private Vector3 lrSelection;

    [Header("Miscellaneous")]
    private bool hexDirections = false;

    private void Update() {

        Vector3 pos = camHolder.transform.position;

        //=====================================================================================DRAGGING
        if (Input.GetMouseButton(0)) {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
            pos.x -= Input.GetAxisRaw("Mouse X") * dragSpeed * ( DragSpeedAmp(yWish) ) * Time.deltaTime;
            pos.z -= Input.GetAxisRaw("Mouse Y") * dragSpeed * ( DragSpeedAmp(yWish) ) * Time.deltaTime;
        } else {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        //================================================================================ZOOMING
        if ( !SSUI.UI_SystemHolder.activeSelf )
            yWish -= Input.mouseScrollDelta.y * scrollSpeed;
        yWish = Mathf.Clamp(yWish, maxZoom, minZoom);
        tiltWish = CamTiltAtY(yWish);
        offsetWish = CamPushBackAtY(yWish);

        //================================================================================SMOOTH THE ZOOMING
        StartCoroutine(SmoothZoom());

        //================================================================================SYSTEM SELECTION VARS
        Ray camRay = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        mPos = Input.mousePosition;
        float dist;

        //================================================================================SYSTEM SELECT DETECTION
        if ( Physics.Raycast(camRay.origin, camRay.direction, out hit, Mathf.Infinity, starSystemHoverLayer ) && PointerIsOnScreen() && !SSUI.UI_SystemHolder.activeSelf && !isChoosingDirection ) {
            GameObject modelGameObject = hit.collider.gameObject;
            hoveredSystem = modelGameObject.GetComponentInParent<StarSystem>() as StarSystem;
            isHovering = true;
        } else {
            isHovering = false;
        }

        //================================================================================SYSTEM SELECT ANIMATION && CAPTURING MOUSE CLICK FOR UI OPEN
        if ( isHovering || SSUI.UI_SystemHolder.activeSelf) {
            hoveredSystem.particle.Play();
            if ( Input.GetMouseButtonDown(0) ) {
                SSUI.sys = hoveredSystem;
                SSUI.OpenUI();
            }
        } else if ( hoveredSystem != null && !isChoosingDirection ) {
            hoveredSystem.particle.Clear();
            hoveredSystem.particle.Stop();
        }

        //================================================================================CHOOSING DIR, AFTER SELECTING TO SEND UNITS
        if ( isChoosingDirection ) {

            SSUI.UI_SystemObstructionTooltip.enabled = false;

            if ( gamePlane.Raycast(camRay, out dist)) {
                OnPlane = camRay.GetPoint(dist);
            }

            Ray targetRay = new Ray(hoveredSystem.transform.position, OnPlane - hoveredSystem.transform.position);
            Debug.DrawRay(targetRay.origin, targetRay.direction*20000, Color.green);
            RaycastHit fromPlanetHit;
            RaycastHit fromCamHit;
            //this massive if says "if we hover and theres a path to said system" basically
            if ( Physics.Raycast(targetRay.origin, targetRay.direction, out fromPlanetHit, 2000f, starSystemLayer) && Physics.Raycast(camRay.origin, camRay.direction, out fromCamHit, Mathf.Infinity, starSystemHoverLayer )  ) {
                if ( fromPlanetHit.collider.transform.parent == fromCamHit.collider.transform.parent ) {
                    targetedSys = fromPlanetHit.collider.gameObject.GetComponentInParent<StarSystem>() as StarSystem;
                } else {
                    SSUI.UI_SystemObstructionTooltip.enabled = true;
                }
            }

            if ( targetedSys == null ) {
                SSUI.UI_SystemPreSelectionTooltip.enabled = true;
            } else {
                SSUI.UI_SystemPreSelectionTooltip.enabled = false;
                SSUI.UI_SystemSelectionTooltip.enabled = true;
            }

            if ( hexDirections ) {
                
            } else {
                lr.SetPosition(0, hoveredSystem.transform.position);
                if ( targetedSys != null ) {
                    StartCoroutine(SmoothSelection(targetedSys.transform.position));
                }
                lr.SetPosition(1, lrSelection);
            }

            //confirming
            if ( Input.GetMouseButton(0) && targetedSys != null ) {
                GameObject armyGO = Instantiate(armyPrefab, hoveredSystem.transform.position + (targetedSys.transform.position - hoveredSystem.transform.position).normalized * 3.2f, Quaternion.LookRotation(targetedSys.transform.position - hoveredSystem.transform.position));
                Army army = armyGO.GetComponent<Army>();

                army.Set(hoveredSystem.unitsToSend, playerID );

                hoveredSystem.particle.Clear();
                hoveredSystem.particle.Stop();

                targetedSys = null;
                isChoosingDirection = false;
                SSUI.UI_SystemHolder.SetActive(false);
                SSUI.UI_SystemSelectionTooltip.enabled = false;
                SSUI.UI_SystemObstructionTooltip.enabled = false;
                Destroy(lr);
                return;
            }

            //cancel
            if ( Input.GetMouseButton(1) ) {
                targetedSys = null;
                isChoosingDirection = false;
                SSUI.UI_SystemHolder.SetActive(false);
                SSUI.UI_SystemSelectionTooltip.enabled = false;
                SSUI.UI_SystemObstructionTooltip.enabled = false;
                Destroy(lr);
                return;
            }
        }

        //================================================================================CAMERA MOVEMENT
        if ( !SSUI.UI_SystemHolder.activeSelf ) {
            camHolder.transform.position = new Vector3(pos.x, yReal, pos.z );
            camHolder.transform.rotation = Quaternion.Euler(tiltReal, 0, 0);
            cam.transform.localPosition = new Vector3(0, 0, offsetReal);
        }
        
    }

    //Handles choosing directions
    public void ChooseDir() {
        isChoosingDirection = true;
        
        lr = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        lrSelection = hoveredSystem.transform.position;
        lr.SetPosition(1, lrSelection);
    }
    
    //returns true when pointer is inside windows
    private bool PointerIsOnScreen() {
        return mPos.y <= Screen.height && mPos.y >= 0 && mPos.x <= Screen.width && mPos.x >= 0; 
    }

    //remaps (-90, 260) to (0.8f, 1.5f)
    private float DragSpeedAmp(float a) {
        return (a + 90) / 200 + 0.1f;
    }

    //remaps (-90, 110+) to (50, 90)
    private float CamTiltAtY(float y) {
        return Mathf.Clamp((y + 90) / 4.5f + 50, 50, 90);
    }

    //remaps(-90, 110+) to (-20, 0)
    private float CamPushBackAtY(float y) {
        return Mathf.Clamp((y + 90) / 9 - 20, -20f, 0f); 
    }

    //smoothes between zoom values
    IEnumerator SmoothZoom() {
        float t = 0;
        float percent = 0;

        float yOld = camHolder.transform.position.y;
        float tiltOld = camHolder.transform.rotation.eulerAngles.x;
        float offsetOld = cam.transform.localPosition.z;

        while ( percent < 1 ) {
            t += Time.deltaTime;
            percent = t * zoomSmoothing;
            yReal = Mathf.Lerp(yOld, yWish, percent);
            tiltReal = Mathf.Lerp(tiltOld, tiltWish, percent);
            offsetReal = Mathf.Lerp(offsetOld, offsetWish, percent);

            yield return null;
        }
    }

    //smoothes between sys selections when sending units
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
