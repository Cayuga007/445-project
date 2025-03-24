using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bone : OVRGrabbable
{
    private Rigidbody rb;
    public float fetchThreshold = 2.0f; // Minimum speed to consider it thrown
    public delegate void BoneThrown(Vector3 position);
    public static event BoneThrown OnBoneThrown;

    private bool hasBeenThrown = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!hasBeenThrown && rb.velocity.magnitude > fetchThreshold)
        {
            hasBeenThrown = true;
            OnBoneThrown?.Invoke(transform.position);
        }
    }
}
