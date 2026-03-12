using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpScript : MonoBehaviour
{
    // followed tutorial: https://youtu.be/pPcYr3tL3Sc?si=i9mida2iDBis4Rsk
    public GameObject player;
    public Transform holdPos;
    //if you copy from below this point, you are legally required to like the video
    public float throwForce = 500f; //force at which the object is thrown at
    public float pickUpRange = 5f; //how far the player can pickup the object from
    private float rotationSensitivity = 1f; //how fast/slow the object is rotated in relation to mouse movement
    private GameObject heldObj; //object which we pick up
    private Rigidbody heldObjRb; //rigidbody of object we pick up
    private bool canDrop = true; //this is needed so we don't throw/drop object when rotating the object
    private int LayerNumber; //layer index

    [Header("Player Scripts")]
    private MonoBehaviour fpcScript;
    private MonoBehaviour fpMovement;

    public List<GameObject> inventory = new List<GameObject>();

    void Start()
    {
        LayerNumber = LayerMask.NameToLayer("holdLayer"); 
        fpcScript = player.GetComponentInChildren<FirstPersonLook>();
        fpMovement = player.GetComponent<FirstPersonMovement>();
        
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObj == null)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange))
                {
                    if (hit.transform.gameObject.tag == "canPickUp" || hit.transform.gameObject.tag == "Phone")
                    {
                        PickUpObject(hit.transform.gameObject);
                    }
                }
            }
            else
            {
                if (canDrop)
                {
                    AddToInventoryAndDrop(); // <-- replaces StopClipping() call
                }
            }
        }

        if (heldObj != null)
        {
            MoveObject();
            RotateObject();
        }
    }

    void AddToInventoryAndDrop()
    {
        inventory.Add(heldObj); 

        fpcScript.enabled = true;
        fpMovement.enabled = true;

        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);

        Destroy(heldObj);
        heldObj = null;
        heldObjRb = null;
    }

    void PickUpObject(GameObject pickUpObj)
    {
        if (pickUpObj.GetComponent<Rigidbody>()) //make sure the object has a RigidBody
        {
            heldObj = pickUpObj; //assign heldObj to the object that was hit by the raycast (no longer == null)
            heldObjRb = pickUpObj.GetComponent<Rigidbody>(); //assign Rigidbody
            heldObjRb.isKinematic = true;
            heldObjRb.transform.parent = holdPos.transform; //parent object to holdposition
            heldObj.layer = LayerNumber; //change the object layer to the holdLayer
            //make sure object doesnt collide with player, it can cause weird bugs
            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), true);
        }
    }
    void MoveObject()
    {
        //keep object position the same as the holdPosition position
        heldObj.transform.position = holdPos.transform.position;
    }
    void RotateObject()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            canDrop = false;
            fpcScript.enabled = false; // freeze camera + movement
            fpMovement.enabled = false;

            float XaxisRotation = Input.GetAxis("Mouse X") * rotationSensitivity;
            float YaxisRotation = Input.GetAxis("Mouse Y") * rotationSensitivity;

            heldObj.transform.Rotate(Vector3.down, XaxisRotation);
            heldObj.transform.Rotate(Vector3.right, YaxisRotation);
        }
        else
        {
            canDrop = true;
        }
    }
    void StopClipping() //function only called when dropping/throwing
    {
        var clipRange = Vector3.Distance(heldObj.transform.position, transform.position); //distance from holdPos to the camera
        //have to use RaycastAll as object blocks raycast in center screen
        //RaycastAll returns array of all colliders hit within the cliprange
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), clipRange);
        //if the array length is greater than 1, meaning it has hit more than just the object we are carrying
        if (hits.Length > 1)
        {
            //change object position to camera position 
            heldObj.transform.position = transform.position + new Vector3(0f, -0.5f, 0f); //offset slightly downward to stop object dropping above player 
            //if your player is small, change the -0.5f to a smaller number (in magnitude) ie: -0.1f
        }
    }
}