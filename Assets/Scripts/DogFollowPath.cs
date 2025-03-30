using UnityEngine;
using UnityEngine.AI;

public class DogGuideController : MonoBehaviour
{
    public Transform player;
    public Transform firstPoint;
    public Transform finalPoint;
    public float triggerDistance = 3f;
    public AudioSource barkAudio; // 拖入 AudioSource

    private NavMeshAgent agent;
    private Animator animator;

    private enum State { Idle, GoToFirst, LookAndBark, WaitForPlayer, GoToFinal, Sit }
    private State currentState = State.Idle;

    private bool hasBarked = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (!agent || !animator || !barkAudio)
        {
            Debug.LogError("Missing component: NavMeshAgent / Animator / AudioSource");
            enabled = false;
            return;
        }

        animator.applyRootMotion = false;
        agent.updateRotation = true;
        agent.updatePosition = true;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        switch (currentState)
        {
            case State.Idle:
                if (distanceToPlayer < triggerDistance)
                {
                    agent.SetDestination(firstPoint.position);
                    currentState = State.GoToFirst;
                }
                break;

            case State.GoToFirst:
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    currentState = State.LookAndBark;
                }
                break;

            case State.LookAndBark:
                LookAtPlayer();

                if (!hasBarked)
                {
                    animator.SetTrigger("BarkTrigger");
                    barkAudio.Play();
                    hasBarked = true;
                    Invoke(nameof(GoToWaitState), 2.5f); // 让狗狗2.5秒后进入下一阶段
                }
                break;

            case State.WaitForPlayer:
                if (distanceToPlayer < triggerDistance)
                {
                    agent.SetDestination(finalPoint.position);
                    currentState = State.GoToFinal;
                }
                break;

            case State.GoToFinal:
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    animator.SetTrigger("SitTrigger");
                    currentState = State.Sit;
                }
                break;

            case State.Sit:
                // 停留坐姿
                break;
        }

        animator.SetBool("isWalking", agent.velocity.magnitude > 0.1f);
    }

    void LookAtPlayer()
    {
        Vector3 lookDirection = player.position - transform.position;
        lookDirection.y = 0;
        if (lookDirection != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2f);
        }
    }

    void GoToWaitState()
    {
        currentState = State.WaitForPlayer;
    }
}
