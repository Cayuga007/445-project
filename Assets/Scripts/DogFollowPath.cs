using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class DogGuideController : MonoBehaviour
{
    public Transform player;
    public Transform firstPoint;
    public Transform finalPoint;
    public float triggerDistance = 3f;
    public AudioSource barkAudio;

    private NavMeshAgent agent;
    private Animator animator;

    private enum State { Idle, GoToFirst, TurnWalk, LookAndBark, WaitForPlayer, GoToFinal, Sit }
    private State currentState = State.Idle;

    private bool hasBarked = false;
    private Vector3 offsetLookPoint;

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
                    // 计算一个偏移点，狗走到这个点可以自然转向玩家
                    Vector3 toPlayer = (player.position - transform.position).normalized;
                    Vector3 sideOffset = Vector3.Cross(toPlayer, Vector3.up) * 1.5f; // 偏右或偏左1.5米
                    offsetLookPoint = transform.position + toPlayer * 1.5f + sideOffset;

                    agent.SetDestination(offsetLookPoint);
                    currentState = State.TurnWalk;
                }
                break;

            case State.TurnWalk:
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    StartCoroutine(LookAtAndBark());
                    currentState = State.LookAndBark;
                }
                break;

            case State.LookAndBark:
                // 吠叫+转身在协程里完成
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
                // 坐下等待
                break;
        }

        animator.SetBool("isWalking", agent.velocity.magnitude > 0.1f);
    }

    IEnumerator LookAtAndBark()
    {
        // 停止移动
        agent.ResetPath();
        animator.SetBool("isWalking", false);

        // 平滑转身
        Quaternion startRot = transform.rotation;
        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0;
        Quaternion targetRot = Quaternion.LookRotation(toPlayer);

        float turnTime = 0.5f;
        float elapsed = 0f;
        while (elapsed < turnTime)
        {
            transform.rotation = Quaternion.Slerp(startRot, targetRot, elapsed / turnTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = targetRot;

        animator.SetTrigger("BarkTrigger");
        barkAudio.Play();

        yield return new WaitForSeconds(2.5f);
        currentState = State.WaitForPlayer;
    }
}
