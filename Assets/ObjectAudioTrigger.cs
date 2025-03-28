using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ObjectAudioTrigger : MonoBehaviour
{
    private AudioSource audioSource;
    private bool hasPlayed = false; // Tracks if audio has already played

    void Start()
    {
        // Search for the AudioSource in child objects
        audioSource = GetComponentInChildren<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource component is missing in children!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the player enters the trigger area
        if (other.CompareTag("Player") && !hasPlayed)
        {
            // Play the audio clip assigned to this object's AudioSource
            if (audioSource != null)
            {
                audioSource.Play();
                hasPlayed = true; // Mark as played
            }
            else
            {
                Debug.LogWarning("AudioSource component is missing!");
            }
        }
    }


}