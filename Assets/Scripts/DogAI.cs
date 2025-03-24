using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DogAI : MonoBehaviour
{
    public Transform player; // Assign player transform
    private Transform bone;
    private NavMeshAgent agent;
    private bool fetching = false;
    private bool returning = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        Bone.OnBoneThrown += StartFetching;
    }

    void StartFetching(Vector3 bonePosition)
    {
        bone = GameObject.FindGameObjectWithTag("Bone").transform;
        agent.SetDestination(bone.position);
        fetching = true;
    }

    void Update()
    {
        if (fetching && Vector3.Distance(transform.position, bone.position) < 1.0f)
        {
            fetching = false;
            returning = true;
            bone.SetParent(transform);
            bone.localPosition = new Vector3(0, 0.5f, 0); // Adjust carry position
            agent.SetDestination(player.position);
        }

        if (returning && Vector3.Distance(transform.position, player.position) < 1.0f)
        {
            returning = false;
            bone.SetParent(null);
            bone.GetComponent<Rigidbody>().isKinematic = false;
            bone.GetComponent<Rigidbody>().velocity = Vector3.zero;
            FindObjectOfType<Day2>().ActionCompleted("fetched");
        }
    }

    void OnDestroy()
    {
        Bone.OnBoneThrown -= StartFetching;
    }

    public void EatBone(GameObject foodBone)
    {
        Destroy(foodBone); // Remove the bone
        FindObjectOfType<Day2>().ActionCompleted("fed");
        Debug.Log("Dog is happy!"); // You can trigger animations or sounds here
    }

}
