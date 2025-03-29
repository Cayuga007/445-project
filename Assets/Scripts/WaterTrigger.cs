using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Swimming");
            other.GetComponentInChildren<OVRCameraRig>().GetComponent<Leaning_Locomotion>().enabled = true;
        }
    }
}
