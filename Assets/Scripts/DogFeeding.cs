using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogFeeding : MonoBehaviour
{
    public DogAI dogAI; // Reference to DogAI for reactions

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FoodBone"))
        {
            dogAI.EatBone(other.gameObject);
        }
    }
}
