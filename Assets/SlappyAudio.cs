using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlappyAudio : MonoBehaviour
{ private bool isGrabbed = false;
    private AudioSource audioSource; // Reference to the AudioSource component

    private void Start()
    {
        // Locate the AudioSource component in the children of this object
        audioSource = GetComponentInChildren<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("No AudioSource found in children of " + gameObject.name);
        }
    }


   

    private void OnTriggerEnter(Collider other)
    {
        
        if (!isGrabbed && (other.gameObject.name == "LeftHandAnchor" || other.gameObject.name == "RightHandAnchor"))
        {
            isGrabbed = true;
            
            audioSource.Play();
        }
    }
}