using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlappyAudio : MonoBehaviour
{ 
    private bool isGrabbed = false;
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
        Debug.Log("Trigger enter");
        Debug.Log(other.gameObject.name);
        if (!isGrabbed && (other.gameObject.name == "LeftHandAnchor" || other.gameObject.name == "RightHandAnchor"))
        {
            Debug.Log("Hand");
            isGrabbed = true;
            
            audioSource.Play();
            FindObjectOfType<Day1>().updateInteraction();
        }
    }

}