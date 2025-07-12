using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomAndLookController : MonoBehaviour
{
    [Header("References")]
    public Camera cam;                // drag First Person Camera
    public Transform lookPivot;       // drag the SAME First Person Camera (or a parent)

    [Header("Look Limits (°)")]
    public float yawLimit = 15f;      // left / right
    public float pitchLimit = 5f;     // up / down
    public float mouseSensitivity = 2f; // mouse sensitivity

    [Header("Zoom Settings (metres)")]
    public float zoomBack = 0.04f;    // how far to pull camera
    public float zoomUp = 0.02f;      // how much to raise camera
    public float zoomTime = 0.08f;    // seconds for the zoom

    /* ───────── private ───────── */
    Vector3 nearPosLocal;             // start close-up
    Vector3 farPosLocal;              // target after zoom
    
    float currentYaw = 0f;            // accumulated yaw rotation
    float currentPitch = 0f;          // accumulated pitch rotation
    
    float zoomTimer = 0f;
    bool zoomDone = true;             // start with zoom done (we're at near position)
    bool mouseLookEnabled = false;    // disable mouse look until zoom starts

    void Start()
    {
        // ❶ place camera close to the monitor
        nearPosLocal = new Vector3(-0.04f, 1.45f, 0.2f);   // tweak if needed
        
        // Force the camera to the desired starting position
        cam.transform.localPosition = nearPosLocal;
        
        // ❂ compute where to end after zoom
        farPosLocal = nearPosLocal + new Vector3(0, zoomUp, -zoomBack);

        // ❸ initialize look angles to zero (centered)
        currentYaw = 0f;
        currentPitch = 0f;
        
        // Set initial rotation to center
        lookPivot.localRotation = Quaternion.identity;
        
        // Don't lock cursor initially - player needs to type
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        /* ── handle zoom interpolation ─────────────────────────── */
        if (!zoomDone)
        {
            zoomTimer += Time.deltaTime / zoomTime;
            cam.transform.localPosition = Vector3.Lerp(nearPosLocal, farPosLocal, zoomTimer);

            if (zoomTimer >= 1f) 
            {
                zoomDone = true;      // finished zoom
                cam.transform.localPosition = farPosLocal; // ensure exact final position
            }
        }

        /* ── handle restricted mouse look ─────────────────────── */
        if (mouseLookEnabled)
        {
            HandleMouseLook();
        }
    }
    
    void HandleMouseLook()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Accumulate rotation
        currentYaw += mouseX;
        currentPitch -= mouseY; // subtract because mouse Y is inverted

        // Clamp the accumulated rotation to limits
        currentYaw = Mathf.Clamp(currentYaw, -yawLimit, yawLimit);
        currentPitch = Mathf.Clamp(currentPitch, -pitchLimit, pitchLimit);

        // Apply the clamped rotation
        lookPivot.localRotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
    }
    
    public void StartZoom()
    {
        zoomTimer = 0f;             // restart the lerp from the near position
        zoomDone = false;
        mouseLookEnabled = true;    // enable mouse look when zoom starts
        
        // Lock cursor for FPS controls
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}