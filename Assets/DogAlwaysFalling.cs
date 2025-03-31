using UnityEngine;

public class DogSwimFloatingFinal : MonoBehaviour
{
    public GameObject dog;

    public float floatSpeed = 1.5f;
    public float floatAmplitudeY = 0.5f; // 上下浮动
    public float floatAmplitudeZ = 0.3f; // 左右浮动

    public float driftSpeed = 0.5f;
    public float driftDistance = 10f;

    private float driftProgress = 0f;
    private bool hasStopped = false;

    private Animator animator;
    private Vector3 startPos;

    public bool isFalling = true;

    void Start()
    {
        if (dog == null)
        {
            Debug.LogError("🐶 请指定 Dog 对象！");
            return;
        }

        animator = dog.GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("🐶 Dog 对象缺少 Animator！");
            return;
        }

        startPos = transform.position;

        if (isFalling)
        {
            animator.SetBool("isFalling", true);
        }
    }

    void Update()
    {
        if (isFalling)
        {
            animator.SetBool("isFalling", true);
        }

        float t = Time.time * floatSpeed;
        float offsetY = Mathf.Sin(t) * floatAmplitudeY;
        float offsetZ = Mathf.Cos(t * 1.2f) * floatAmplitudeZ;

        if (!hasStopped)
        {
            driftProgress -= driftSpeed * Time.deltaTime;

            if (Mathf.Abs(driftProgress) >= driftDistance)
            {
                driftProgress = -driftDistance;
                hasStopped = true;
            }
        }

        transform.position = startPos + new Vector3(driftProgress, offsetY, offsetZ);
    }
}
