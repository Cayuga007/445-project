using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public float interactionDistance = 3f; // Maximum distance for interaction
    public Transform holdPoint; // Point where the object will be held
    private GameObject heldObject = null; // Currently held object
    private Vector3 originalPosition; // Original position of the object
    private Quaternion originalRotation; // Original rotation of the object

    [Header("Raycast Settings")]
    public LayerMask interactableLayer; // Layer for interactable objects
    public Color rayColor = Color.red; // Color of the debug ray

    [Header("Audio Settings")]
    public AudioSource audioSource; // Audio source for playing sounds
    public AudioClip defaultClip; // Default audio clip when interacting

    [Header("OVR Input Settings")]
    public OVRInput.Button interactionButton = OVRInput.Button.PrimaryIndexTrigger; // Button to interact

    void Update()
    {
        HandleInteraction();
        VisualizeRaycast();
    }

    private void HandleInteraction()
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

                // Move object to hold point and center it
                heldObject.transform.position = holdPoint.position;
                heldObject.transform.SetParent(holdPoint);

                // Play audio specific to the object or fallback to default clip
                AudioSource objectAudioSource = heldObject.GetComponent<AudioSource>();
                if (objectAudioSource != null && objectAudioSource.clip != null)
                {
                    objectAudioSource.Play();
                }
                else if (audioSource != null && defaultClip != null)
                {
                    audioSource.clip = defaultClip;
                    audioSource.Play();
                }
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

    private void VisualizeRaycast()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * interactionDistance, rayColor); // Visualize the ray in Scene view
    }
}