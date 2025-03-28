using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableObject : MonoBehaviour
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
        if (other.gameObject.name == "EatHitbox")
        {
            other.transform.parent.GetComponent<DogAI>().eat();
            Destroy(gameObject);
        }
    }
}
