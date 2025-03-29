using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OVRCameraController : MonoBehaviour
{
    public float moveSpeed = 3.0f;
    public float mouseSensitivity = 2.0f;
    private float verticalRotation = 0.0f;

    public Transform playerBody; // Assign your VR Rig or Player Object here

    void Start()
    {
        // Initially lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;  // Hide the cursor initially
    }

    void Update()
    {
        // Unlock the cursor if the player clicks
        if (Input.GetMouseButtonDown(0)) // Left mouse click to unlock
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true; // Show the cursor
        }

        // Movement using WASD
        float horizontal = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float vertical = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        playerBody.Translate(new Vector3(horizontal, 0, vertical));

        // Mouse Look for Rotation
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        // Apply up/down rotation to the camera only
        transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);

        // Apply left/right rotation to the player's body
        playerBody.Rotate(Vector3.up * mouseX);
    }
}