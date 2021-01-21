using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
    
    private Camera cam;
    private Rigidbody rb;

    [Header("Activation Area")]
    public float xThreshold = 0.1f;
    public float yThreshold = 0.1f;
    public bool yRespectsAspectRatio = false;

    [Header("Mouse Settings")]
    public float dragSpeed;
    public float scrollSpeed;
    public bool lockCursor = false;

    [Range(0.001f,10f)]
    public float zoomSmoothing;
    [Range(0.001f, 50f)]
    //public float lateralSmoothing;
    private float yVal;
    private float yZoom;
    private readonly float maxZoom = -90f;
    private readonly float minZoom = 260f;

    private void Awake() {
        cam = FindObjectOfType<Camera>();
        if ( yRespectsAspectRatio )
            yThreshold = xThreshold * 0.5625f; //16:9 conversion ratio
        if ( lockCursor )
            Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update() {

        Vector3 pos = cam.transform.position;
        
        //zooming
        yVal -= Input.mouseScrollDelta.y * scrollSpeed;
        yVal = Mathf.Clamp(yVal, maxZoom, minZoom);

        //dragging
        if (Input.GetMouseButton(0)) {
            Cursor.visible = false;
            pos.x -= Input.GetAxisRaw("Mouse X") * dragSpeed * ( DragSpeedAmp(yVal) ) * Time.deltaTime;
            pos.z -= Input.GetAxisRaw("Mouse Y") * dragSpeed * ( DragSpeedAmp(yVal) ) * Time.deltaTime;
        } else {
            Cursor.visible = true;
        }

        StartCoroutine(Smoother());

        cam.transform.position = new Vector3(pos.x, yZoom, pos.z);
    }

    IEnumerator Smoother() {
        float t = 0;
        float percent = 0;

        while ( percent < 1 ) {
            t += Time.deltaTime;
            percent = t * zoomSmoothing;
            yZoom = Mathf.Lerp(cam.transform.position.y, yVal, percent);
            yield return null;
        }
    }

    //0.8 at max zoom, 1.5 at min zoom
    private float DragSpeedAmp(float a) {
        return (a + 90) / 250 + 0.1f;
    }

}
