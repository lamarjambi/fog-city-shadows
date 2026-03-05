using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ZoomAndLookController : MonoBehaviour
{
    [Header("References")]
    public Camera cam;                // drag First Person Camera
    public Transform lookPivot;       // drag the SAME First Person Camera (or a parent)
    public TMP_Text leavePromptText;  // drag a UI Text element for "Leave?" prompt
    public GameObject leaveBackground; // drag the background panel here

    [Header("Look Limits (°)")]
    public float yawLimit = 15f;      // left / right
    public float pitchLimit = 5f;     // up / down
    public float mouseSensitivity = 2f; // mouse sensitivity

    [Header("Zoom Settings (metres)")]
    public float zoomBack = 0.04f;    // how far to pull camera
    public float zoomUp = 0.02f;      // how much to raise camera
    public float zoomTime = 0.08f;    // seconds for the zoom

    [Header("Scene Transition")]
    public float delayBeforePrompt = 7f; // seconds to wait after zoom before show  ing prompt
    public string nextSceneName = "GameScene"; // name of the scene to load
    public Animator jumpscareAnimator; // drag jumpscare animator here
    
    /* ───────── private ───────── */
    Vector3 nearPosLocal;             // start close-up
    Vector3 farPosLocal;              // target after zoom
    
    float currentYaw = 0f;            // accumulated yaw rotation
    float currentPitch = 0f;          // accumulated pitch rotation
    
    float zoomTimer = 0f;
    bool zoomDone = true;             // start with zoom done (we're at near position)
    bool mouseLookEnabled = false;    // disable mouse look until zoom starts

    // New variables for scene transition
    float promptTimer = 0f;
    bool promptShown = false;
    bool waitingForInput = false;

    void Start()
    {
        // ❶ place camera close to the monitor
        nearPosLocal = new Vector3(-0.04f, 1.62f, 0.45f);   // tweak if needed
        
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
        
        // Hide leave prompt and background initially
        if (leavePromptText != null)
            leavePromptText.gameObject.SetActive(false);
        if (leaveBackground != null)
            leaveBackground.SetActive(false);
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
                promptTimer = 0f;     // start timer for leave prompt
            }
        }

        /* ── handle prompt timer after zoom ─────────────────────── */
        if (zoomDone && !promptShown && mouseLookEnabled)
        {
            promptTimer += Time.deltaTime;
            if (promptTimer >= delayBeforePrompt)
            {
                ShowLeavePrompt();
            }
        }

        /* ── handle scene transition input ─────────────────────── */
        if (waitingForInput)
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                HandleYesResponse();
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
                HandleNoResponse();
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
    
    void ShowLeavePrompt()
    {
        if (leavePromptText != null)
        {
            leavePromptText.text = "Leave?\n<color=white>Y</color> / <color=white>N</color>";
            leavePromptText.gameObject.SetActive(true);
        }
        
        if (leaveBackground != null)
        {
            leaveBackground.SetActive(true);
        }
        
        promptShown = true;
        waitingForInput = true;
        
        // Keep cursor locked and hidden
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void HandleYesResponse()
    {
        // Change Y to yellow and load scene
        if (leavePromptText != null)
        {
            leavePromptText.text = "Leave?\n<color=yellow>Y</color> / <color=white>N</color>";
        }
        
        waitingForInput = false;
        
        // Small delay to show the color change, then load scene
        StartCoroutine(LoadSceneAfterDelay(0.5f));
    }
    
    void HandleNoResponse()
    {
        // Change N to yellow and trigger jumpscare
        if (leavePromptText != null)
        {
            leavePromptText.text = "Leave?\n<color=white>Y</color> / <color=yellow>N</color>";
        }
        
        waitingForInput = false;
        
        // Small delay to show the color change, then jumpscare
        StartCoroutine(TriggerJumpscareAfterDelay(0.5f));
    }
    
    IEnumerator LoadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadNextScene();
    }
    
    IEnumerator TriggerJumpscareAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        TriggerJumpscare();
    }
    
    void TriggerJumpscare()
    {
        // Hide the leave prompt
        if (leavePromptText != null)
            leavePromptText.gameObject.SetActive(false);
        if (leaveBackground != null)
            leaveBackground.SetActive(false);
            
        // Play jumpscare animation
        if (jumpscareAnimator != null)
        {
            jumpscareAnimator.SetTrigger("StartJumpscare");
        }
        
        // Optional: Re-enable mouse look for jumpscare effect
        mouseLookEnabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void LoadNextScene()
    {
        // Optional: Add fade out or loading screen here
        SceneManager.LoadScene(nextSceneName);
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