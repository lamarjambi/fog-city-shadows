using UnityEngine;

public class FirstPersonLook : MonoBehaviour
{
    [SerializeField] Transform character;
    public float sensitivity = 2;
    public float smoothing   = 1.5f;

    Vector2 velocity;        // (x = yaw, y = pitch)
    Vector2 frameVelocity;

    // NEW — cache the starting yaw that’s already on the character
    float initialYaw;

    void Reset()
    {
        character = GetComponentInParent<FirstPersonMovement>().transform;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // --- NEW LINES -----------------------------------------
        // read the Y-rotation you set in the Inspector (e.g., 180°)
        initialYaw        = character.localEulerAngles.y;
        velocity.x        = initialYaw;           // pre-seed yaw
        // --------------------------------------------------------
    }

    void Update()
    {
        Vector2 mouseDelta      = new Vector2(Input.GetAxisRaw("Mouse X"),
                                              Input.GetAxisRaw("Mouse Y"));
        Vector2 rawFrameVelocity = Vector2.Scale(mouseDelta,
                                                 Vector2.one * sensitivity);
        frameVelocity = Vector2.Lerp(frameVelocity,
                                     rawFrameVelocity,
                                     1 / smoothing);
        velocity += frameVelocity;
        velocity.y = Mathf.Clamp(velocity.y, -90, 90);

        transform.localRotation  = Quaternion.AngleAxis(-velocity.y, Vector3.right);
        character.localRotation  = Quaternion.AngleAxis(velocity.x,  Vector3.up);
    }
}
