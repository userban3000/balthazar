using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
    
    private Camera cam;
    private Rigidbody rb;

    [Header("UI")]
    public StarSystemUI SSUI;

    [Header("Mouse Settings")]
    public float dragSpeed;
    public float scrollSpeed;
    [Range(0.001f,10f)]
    public float zoomSmoothing;

    private float yVal;
    private float yZoom;
    private readonly float maxZoom = -90f;
    private readonly float minZoom = 260f;

    private void Awake() {
        cam = GetComponent<Camera>();
    }

    private void Update() {

        Vector3 pos = cam.transform.position;
        
        //zooming
        if ( !SSUI.UI_Canvas.enabled )
            yVal -= Input.mouseScrollDelta.y * scrollSpeed;
        yVal = Mathf.Clamp(yVal, maxZoom, minZoom);

        //dragging
        if (Input.GetMouseButton(0)) {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
            pos.x -= Input.GetAxisRaw("Mouse X") * dragSpeed * ( DragSpeedAmp(yVal) ) * Time.deltaTime;
            pos.z -= Input.GetAxisRaw("Mouse Y") * dragSpeed * ( DragSpeedAmp(yVal) ) * Time.deltaTime;
        } else {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        StartCoroutine(SmoothZoom());

        if ( !SSUI.UI_Canvas.enabled )
            cam.transform.position = new Vector3(pos.x, yZoom, pos.z);
    }

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

    //remaps (-90, 260) to (0.8f, 1.5f)
    private float DragSpeedAmp(float a) {
        return (a + 90) / 200 + 0.1f;
    }

}
