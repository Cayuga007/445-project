using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ObjectPickup : MonoBehaviour
{
    public float interactionDistance = 3f; // Maximum distance for interaction
    public Transform holdPoint; // Point where the object will be held
    private GameObject heldObject = null; // Currently held object
    private Vector3 originalPosition; // Original position of the object
    private Quaternion originalRotation; // Original rotation of the object

    public OVRInput.Button interactionButton = OVRInput.Button.PrimaryIndexTrigger; // Button to interact (e.g., trigger button)

    void Update()
    {
        if (OVRInput.GetDown(interactionButton)) // Detect button press on Oculus controller
        {
            if (heldObject == null)
            {
                TryPickupObject();
            }
            else
            {
                PlaceObjectDown();
            }
        }
    }

    private void TryPickupObject()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward); // Raycast from headset camera
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
        {
            if (hit.collider.CompareTag("Interactable")) // Check if object has "Interactable" tag
            {
                heldObject = hit.collider.gameObject;

                // Save original position and rotation
                originalPosition = heldObject.transform.position;
                originalRotation = heldObject.transform.rotation;

                // Disable physics while holding
                Rigidbody rb = heldObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                }

                // Move object to hold point and center it
                heldObject.transform.position = holdPoint.position;

                // Center adjustment (optional offset)
                Vector3 offset = new Vector3(0, 0, 0); // Adjust as needed
                heldObject.transform.position += offset;

                // Parent it to the hold point
                heldObject.transform.SetParent(holdPoint);
            }
        }
    }

    private void PlaceObjectDown()
    {
        if (heldObject != null)
        {
            // Re-enable physics
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }

            // Unparent the object and return it to its original position and rotation
            heldObject.transform.SetParent(null);
            heldObject.transform.position = originalPosition;
            heldObject.transform.rotation = originalRotation;

            heldObject = null;
        }
    }
}