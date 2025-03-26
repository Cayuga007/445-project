using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // Required for NavMeshAgent

public class DogFollowPath : MonoBehaviour
{
    public Transform targetPoint; // The point where the dog should go
    public Transform player; // Reference to OVRPlayerController
    public GameEndFade gameEndFade; // Assign in Inspector
    public float maxDistance = 5f; // Max distance player can be behind
    public float stopThreshold = 1.5f; // Distance at which dog considers itself "arrived"

    private NavMeshAgent agent;
    private bool destinated_reached = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (targetPoint != null)
            agent.SetDestination(targetPoint.position);
    }

    void Update()
    {
        if (destinated_reached) return;

        float playerDistance = Vector3.Distance(transform.position, player.position);
        float targetDistance = Vector3.Distance(transform.position, targetPoint.position);

        // If the player is too far behind, stop moving
        if (playerDistance > maxDistance)
        {
            agent.isStopped = true;
        }
        else
        {
            // Resume moving if the player is close enough
            agent.isStopped = false;
            agent.SetDestination(targetPoint.position);
        }

        // If the dog reaches the target, stop
        if (targetDistance < stopThreshold)
        {
            agent.isStopped = true;
            destinated_reached = true;

            // Start fade only once
            if (!gameEndFade.IsInvoking("StartGameEndSequence"))
            {
                gameEndFade.Invoke("StartGameEndSequence", 2f);
            }
        }
    }
}
