using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MirrorEffect : MonoBehaviour
{
    public Material blankMaterial; // The default blank mirror material
    public Material faceMaterial;  // The material with the MC's face
    public Renderer mirrorRenderer; // Drag your mirror object here

    private void Start()
    {
        if (mirrorRenderer == null)
            mirrorRenderer = GetComponent<Renderer>();

        mirrorRenderer.material = blankMaterial;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the OVR Player Controller
        if (other.CompareTag("Player")) // Ensure your OVR Player Controller has the "Player" tag
        {
            mirrorRenderer.material = faceMaterial;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Revert to blank material when the player exits the trigger
        if (other.CompareTag("Player"))
        {
            mirrorRenderer.material = blankMaterial;
        }
    }
}