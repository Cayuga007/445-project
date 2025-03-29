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
        if (objectsInteracted == 3){
            audioSource.Play();
            StartCoroutine(WaitForAudio());
        }
    }

   private System.Collections.IEnumerator WaitForAudio()
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        FindObjectOfType<SceneController>().LoadNextScene();
    }
}
