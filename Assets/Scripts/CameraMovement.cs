using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
    
    private Camera cam;

    [Header("Activation Area")]
    public float xThreshold = 0.1f;
    public float yThreshold = 0.1f;
    public bool yRespectsAspectRatio = false;

    [Header("Mouse Settings")]
    public float moveSens = 1f;
    public bool lockCursor = false;

    private void Awake() {
        cam = FindObjectOfType<Camera>();
        if ( yRespectsAspectRatio )
            yThreshold = xThreshold * 0.5625f; //16:9 conversion ratio
        if ( lockCursor )
            Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update() {
        Vector3 mPos = Input.mousePosition;

        mPos.x /= Screen.width;
        mPos.y /= Screen.height;

        Vector3 move = Vector3.zero;

        //rewrite this trash again later
        if ( mPos.x <= xThreshold ) 
            move.x = -moveSens * Mathf.Pow( (xThreshold - mPos.x) * ( 1 / xThreshold ), 6); //move left
        if ( mPos.x >= 1 - xThreshold ) 
            move.x = moveSens * Mathf.Pow( (1/xThreshold) * (mPos.x - 1 + xThreshold), 6); // move right
        if ( mPos.y <= yThreshold )
            move.z = -moveSens * Mathf.Pow( (yThreshold - mPos.y) * ( 1 / yThreshold ), 6); //move down
        if ( mPos.y >= 1 - yThreshold ) 
            move.z = moveSens * Mathf.Pow((1/yThreshold) * (mPos.y - 1 + yThreshold), 6); // move up

        if ( move != Vector3.zero ) {
            cam.transform.position += move * Time.deltaTime;
        }
    }

}
