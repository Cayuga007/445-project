using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogAI : MonoBehaviour
{
    private Animator animator;
    private bool isSitting = false;
    private bool eaten = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isSitting && (other.gameObject.name == "LeftHandAnchor" || other.gameObject.name == "RightHandAnchor"))
        {
            isSitting = true;
            animator.SetTrigger("SitTrigger");
            updateDay();
        }
    }

    public void eat()
    {
        if (eaten) return;

        eaten = true;
        animator.SetTrigger("EatTrigger");
        updateDay();
    }

    private void updateDay()
    {
        if (isSitting && eaten)
            FindObjectOfType<SceneController>().LoadNextScene();
    }
}
