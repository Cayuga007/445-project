using UnityEngine;

public class DogAnimationController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        InvokeRepeating("PlayRandomAction", 2f, 5f); // 每5秒执行一次新动作
    }

    void PlayRandomAction()
    {
        int randomValue = Random.Range(1, 4); // 1-3 的随机值
        animator.SetInteger("RandomAction", randomValue);
        Debug.Log("随机动画: " + randomValue);
    }

}
