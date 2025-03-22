using UnityEngine;

public class DogAnimationController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        InvokeRepeating("PlayRandomAction", 2f, 5f); // ÿ5��ִ��һ���¶���
    }

    void PlayRandomAction()
    {
        int randomValue = Random.Range(1, 4); // 1-3 �����ֵ
        animator.SetInteger("RandomAction", randomValue);
        Debug.Log("�������: " + randomValue);
    }

}
