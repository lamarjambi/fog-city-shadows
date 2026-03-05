using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ZoomAndLookController : MonoBehaviour
{
    [Header("References")]
    public Camera cam;                
    public Transform lookPivot;      
    public TMP_Text leavePromptText;  
    public GameObject leaveBackground; 

    [Header("Look Limits (°)")]
    public float yawLimit = 15f;      
    public float pitchLimit = 5f;     
    public float mouseSensitivity = 2f; 

    [Header("Zoom Settings (metres)")]
    public float zoomBack = 0.04f;    
    public float zoomUp = 0.02f;      
    public float zoomTime = 0.08f;    

    [Header("Scene Transition")]
    public float delayBeforePrompt = 7f; 
    public string nextSceneName = "GameScene"; 
    public Animator jumpscareAnimator; 

    Vector3 nearPosLocal;             
    Vector3 farPosLocal;              
    
    float currentYaw = 0f;            
    float currentPitch = 0f;          
    
    float zoomTimer = 0f;
    bool zoomDone = true;             
    bool mouseLookEnabled = false;    
    
    float promptTimer = 0f;
    bool promptShown = false;
    bool waitingForInput = false;

    void Start()
    {
        nearPosLocal = new Vector3(-0.04f, 1.62f, 0.45f);  
        
        cam.transform.localPosition = nearPosLocal;
        
        farPosLocal = nearPosLocal + new Vector3(0, zoomUp, -zoomBack);

        currentYaw = 0f;
        currentPitch = 0f;
        
        lookPivot.localRotation = Quaternion.identity;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        if (leavePromptText != null)
            leavePromptText.gameObject.SetActive(false);
        if (leaveBackground != null)
            leaveBackground.SetActive(false);
    }

    void Update()
    {
        if (!zoomDone)
        {
            zoomTimer += Time.deltaTime / zoomTime;
            cam.transform.localPosition = Vector3.Lerp(nearPosLocal, farPosLocal, zoomTimer);

            if (zoomTimer >= 1f) 
            {
                zoomDone = true;      
                cam.transform.localPosition = farPosLocal; 
                promptTimer = 0f;     
            }
        }

        if (zoomDone && !promptShown && mouseLookEnabled)
        {
            promptTimer += Time.deltaTime;
            if (promptTimer >= delayBeforePrompt)
            {
                ShowLeavePrompt();
            }
        }

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

        if (mouseLookEnabled)
        {
            HandleMouseLook();
        }
    }
    
    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        currentYaw += mouseX;
        currentPitch -= mouseY; 

        currentYaw = Mathf.Clamp(currentYaw, -yawLimit, yawLimit);
        currentPitch = Mathf.Clamp(currentPitch, -pitchLimit, pitchLimit);

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
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void HandleYesResponse()
    {
        if (leavePromptText != null)
        {
            leavePromptText.text = "Leave?\n<color=yellow>Y</color> / <color=white>N</color>";
        }
        
        waitingForInput = false;
        
        StartCoroutine(LoadSceneAfterDelay(0.5f));
    }
    
    void HandleNoResponse()
    {
        // :func: if player answers no -> jumpscare
        if (leavePromptText != null)
        {
            leavePromptText.text = "Leave?\n<color=white>Y</color> / <color=yellow>N</color>";
        }
        
        waitingForInput = false;
        
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
        if (leavePromptText != null)
            leavePromptText.gameObject.SetActive(false);
        if (leaveBackground != null)
            leaveBackground.SetActive(false);
            
        if (jumpscareAnimator != null)
        {
            jumpscareAnimator.SetTrigger("StartJumpscare");
        }
        
        mouseLookEnabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
    
    public void StartZoom()
    {
        zoomTimer = 0f;           
        zoomDone = false;
        mouseLookEnabled = true;    
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}