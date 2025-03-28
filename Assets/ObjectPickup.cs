using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPickup : MonoBehaviour
{
    [Header("Hand Settings")]
    public OVRHand ovrHand; // Reference to the OVRHand component
    public OVRHand.HandFinger pinchFinger = OVRHand.HandFinger.Index; // Finger used for pinching
    public float pinchThreshold = 0.8f; // Pinch strength threshold for grabbing

    [Header("Pickup Settings")]
    public float interactionDistance = 3f; // Maximum distance for interaction
    public Transform holdPoint; // Point where grabbed objects will be held
    private GameObject heldObject = null; // Currently held object
    private Vector3 originalPosition; // Original position of the object
    private Quaternion originalRotation; // Original rotation of the object

    [Header("Layer Settings")]
    public LayerMask interactableLayer; // Layer for interactable objects

    void Update()
    {
        HandleInteraction();
    }

    private void HandleInteraction()
    {
        if (ovrHand.GetFingerPinchStrength(pinchFinger) > pinchThreshold) // Detect pinch gesture
        {
            if (heldObject == null)
            {
                TryPickupObject();
            }
        }
        else if (heldObject != null && ovrHand.GetFingerPinchStrength(pinchFinger) <= pinchThreshold) // Release when pinch is below threshold
        {
            PlaceObjectDown();
        }
    }

    private void TryPickupObject()
    {
        Ray ray = new Ray(ovrHand.PointerPose.position, ovrHand.PointerPose.forward); // Raycast from hand pointer pose
        Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.green); // Visualize ray in Scene view

        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactableLayer))
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

                // Move object to hold point and parent it to the hand
                heldObject.transform.position = holdPoint.position;
                heldObject.transform.rotation = holdPoint.rotation;
                heldObject.transform.SetParent(holdPoint);
            }
        }
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
}