using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public Transform holdPoint; // Point where the object will be held
    private GameObject heldObject = null; // Currently held object
    private Vector3 originalPosition; // Original position of the object
    private Quaternion originalRotation; // Original rotation of the object

    [Header("Layer Settings")]
    public LayerMask interactableLayer; // Layer for interactable objects

    [Header("Input Settings")]
    public OVRInput.Button pickupButton = OVRInput.Button.One; // Default button (A button)

    private GameObject nearbyObject = null; // Object currently in collision range

    void Update()
    {
        HandleInteraction();
    }

    private void HandleInteraction()
    {
        if (OVRInput.Get(pickupButton)) // Button is held down
        {
            if (heldObject == null && nearbyObject != null) // Pick up nearby object
            {
                TryPickupObject(nearbyObject);
            }
        }
        else if (heldObject != null) // Release object when button is released
        {
            PlaceObjectDown();
        }
    }

    private void TryPickupObject(GameObject obj)
    {
        heldObject = obj;

        // Save original position and rotation
        originalPosition = heldObject.transform.position;
        originalRotation = heldObject.transform.rotation;

        // Disable physics while holding
        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        // Move object to hold point and parent it to the hand
        heldObject.transform.position = holdPoint.position;
        heldObject.transform.rotation = holdPoint.rotation;
        heldObject.transform.SetParent(holdPoint);
    }

    private void PlaceObjectDown()
    {
        if (heldObject != null)
        {
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }

            // Unparent the object and restore its original position/rotation
            heldObject.transform.SetParent(null);
            heldObject.transform.position = originalPosition;
            heldObject.transform.rotation = originalRotation;

            heldObject = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable")) // Check if object is interactable
        {
            nearbyObject = other.gameObject; // Store reference to nearby object
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == nearbyObject) // Clear reference when exiting collision range
        {
            nearbyObject = null;
        }
    }
}