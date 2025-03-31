using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Day1 : MonoBehaviour
{
    private int objectsInteracted = 0;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void updateInteraction()
    {
        objectsInteracted++;
        if (objectsInteracted == 3)
        {
            StartCoroutine(DelayedAudio());
        }
    }

    private IEnumerator DelayedAudio()
    {
        yield return new WaitForSeconds(5f); // Wait for 5 seconds before playing audio
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length);
        FindObjectOfType<SceneController>().LoadNextScene();
    }
}