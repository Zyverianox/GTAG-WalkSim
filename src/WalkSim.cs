using BepInEx;
using UnityEngine;

[BepInPlugin("com.yourname.walksim", "WalkSim Mod", "0.0.1")]
public class WalkSim : BaseUnityPlugin
{
    private Transform playerTransform;
    private float moveSpeed = 3f;
    private float mouseSensitivity = 2f;
    private float verticalLookRotation = 0f;

    void Start()
    {
        // Lock and hide the cursor so mouse movement controls the camera
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Try to find the player object by tag — adjust if Gorilla Tag uses something else
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Logger.LogError("Player object not found! Check the tag or how to get player in Gorilla Tag.");
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        HandleMovement();
        HandleMouseLook();
        HandleJump();
    }

    private void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal"); // A/D keys
        float z = Input.GetAxis("Vertical");   // W/S keys

        Vector3 moveDirection = playerTransform.forward * z + playerTransform.right * x;
        moveDirection.y = 0;

        playerTransform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotate the player left/right
        playerTransform.Rotate(Vector3.up * mouseX);

        // Rotate the camera up/down and clamp it
        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        if (Camera.main != null)
        {
            Camera.main.transform.localRotation = Quaternion.Euler(verticalLookRotation, 0, 0);
        }
        else
        {
            Logger.LogWarning("Main camera not found!");
        }
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Rigidbody rb = playerTransform.GetComponent<Rigidbody>();
            if (rb != null && Mathf.Abs(rb.velocity.y) < 0.01f) // Simple grounded check
            {
                rb.AddForce(Vector3.up * 5f, ForceMode.VelocityChange);
            }
        }
    }
}