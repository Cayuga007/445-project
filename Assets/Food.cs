using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    private bool isGrabbed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isGrabbed && (other.gameObject.name == "LeftHandAnchor" || other.gameObject.name == "RightHandAnchor"))
        {
            isGrabbed = true;
            transform.SetParent(other.gameObject.transform.Find("GrabPoint"));
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        if (!isGrabbed && other.gameObject.name == "Dog")
        {
            Destroy(this);
        }
    }
}
