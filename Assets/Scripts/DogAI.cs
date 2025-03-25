using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DogAI : MonoBehaviour
{
    private Vector3 originalPosition; // Where the dog started
    private Rigidbody rb;
    private bool chasingBone = false;
    private Transform boneTransform;
    private float chaseSpeed = 5f; // Speed at which the dog moves
    private float grabDistance = 1.0f; // Distance at which the dog grabs the bone
    private NavMeshAgent navAgent;

    // New variables for feeding behavior
    private bool fed = false;
    public GameObject foodBone; // Reference to the FoodBone

    void Start()
    {
        // Store the dog's original position
        originalPosition = transform.position;

        // Get NavMeshAgent component for navigation (if using NavMesh)
        navAgent = GetComponent<NavMeshAgent>();

        // Subscribe to the OnBoneThrown event
        Bone.OnBoneThrown += StartChasingBone;
    }

    void OnDestroy()
    {
        // Unsubscribe when the dog is destroyed
        Bone.OnBoneThrown -= StartChasingBone;
    }

    void Update()
    {
        if (chasingBone && boneTransform != null)
        {
            // Chase the bone by moving towards it
            if (navAgent != null)
            {
                navAgent.SetDestination(boneTransform.position);
            }
            else
            {
                // If not using NavMesh, manually move toward the bone
                transform.position = Vector3.MoveTowards(transform.position, boneTransform.position, chaseSpeed * Time.deltaTime);
            }

            // If the dog is close enough to the bone, grab it and return to the starting position
            if (Vector3.Distance(transform.position, boneTransform.position) < grabDistance)
            {
                GrabBone();
            }
        }

        // Check if the dog is feeding (if it has the FoodBone and is close to it)
        if (!fed && foodBone != null && Vector3.Distance(transform.position, foodBone.transform.position) < 1.0f)
        {
            fed = true;

            Animator dogAnimator = GetComponent<Animator>();
            if (dogAnimator != null)
            {
                dogAnimator.SetTrigger("Eat");
            }

            Destroy(foodBone);
            FindObjectOfType<Day2>().ActionCompleted("fed");
        }
    }

    // This method starts the chasing behavior once the bone is thrown
    private void StartChasingBone(Vector3 bonePosition)
    {
        // Find the bone in the scene (you can also cache the reference to the bone for better performance)
        GameObject boneObject = GameObject.FindWithTag("Bone");
        if (boneObject != null)
        {
            boneTransform = boneObject.transform;
            chasingBone = true;
        }
    }

    // This method simulates the dog grabbing the bone and returning to its starting position
    private void GrabBone()
    {
        // Simulate grabbing the bone (attach it to the dog)
        boneTransform.position = transform.position;  // Move the bone to the dog's position (mouth, hand, etc.)
        boneTransform.SetParent(transform);  // Attach the bone to the dog so it moves with the dog

        // Now the dog is ready to return to its original position
        StartCoroutine(ReturnToOriginalPosition());
    }

    // Coroutine to return the dog to its original position after grabbing the bone
    private IEnumerator ReturnToOriginalPosition()
    {
        chasingBone = false; // Stop chasing the bone
        navAgent.SetDestination(originalPosition);  // Set destination to original position

        // Wait until the dog reaches its starting position
        while (Vector3.Distance(transform.position, originalPosition) > 0.5f)
        {
            yield return null;  // Wait until the dog reaches the position
        }

        // Once the dog reaches the original position, detach the bone
        boneTransform.SetParent(null);  // Detach the bone from the dog
        boneTransform.GetComponent<Rigidbody>().isKinematic = false;  // Allow physics interaction again

        FindObjectOfType<Day2>().ActionCompleted("fetched");
    }
}
