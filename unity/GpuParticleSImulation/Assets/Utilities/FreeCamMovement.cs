using UnityEngine;

public class FreeCamMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float movementSpeed = 10f; // Base movement speed
    public float boostMultiplier = 2f; // Speed multiplier when boosting

    [Header("Mouse Settings")]
    public float lookSensitivity = 2.0f; // Mouse look sensitivity
    public bool invertY = false; // Invert Y-axis mouse control

    private float yaw = 0f;
    private float pitch = 0f;

    private bool isCursorLocked = false;

    private void Start()
    {
        LockCursor(false);

        // Initialize the yaw and pitch with the current rotation
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    private void Update() 
    {
        HandleCursorToggle();

        if (isCursorLocked)
        {
            HandleMouseLook();
            HandleMovement();
        }
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        yaw += mouseX;
        pitch -= (invertY ? -mouseY : mouseY);
        pitch = Mathf.Clamp(pitch, -89f, 89f); // Clamp pitch to avoid flipping

        transform.eulerAngles = new Vector3(pitch, yaw, 0f);
    }

    private void HandleMovement()
    {
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) direction += transform.forward;
        if (Input.GetKey(KeyCode.S)) direction -= transform.forward;
        if (Input.GetKey(KeyCode.A)) direction -= transform.right;
        if (Input.GetKey(KeyCode.D)) direction += transform.right;
        if (Input.GetKey(KeyCode.E)) direction += transform.up;
        if (Input.GetKey(KeyCode.Q)) direction -= transform.up;

        float currentSpeed = movementSpeed;

        // Boost multiplier with Left Shift key
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed *= boostMultiplier;
        }

        transform.position += direction * currentSpeed * Time.deltaTime;
    }

    private void HandleCursorToggle()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isCursorLocked = !isCursorLocked;
            LockCursor(isCursorLocked);
        }
    }

    private void LockCursor(bool lockCursor)
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}