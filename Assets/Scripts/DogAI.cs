using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogAI : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("INTERACT");
        if (other.gameObject.name == "OVRCustomHandPrefab_L" || other.gameObject.name == "OVRCustomHandPrefab_R")
        {
            //animator.SetTrigger("Pet");
            Debug.Log("Pet");
        }
    }
}
