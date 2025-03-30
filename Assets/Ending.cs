using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereTrigger : MonoBehaviour
{
    // This function will be called when another collider enters the trigger zone
    private void OnTriggerEnter(Collider other)
    {
        // Check if the player (you can use the tag or a specific object) touched the sphere
        if (other.CompareTag("Player"))
        {
            FindObjectOfType<SceneController>().LoadNextScene();
        }
    }
}
