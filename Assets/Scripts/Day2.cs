using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Day2 : MonoBehaviour
{
    private bool hasFed = false;
    private bool hasFetched = false;

    public void ActionCompleted(string action)
    {
        if (action == "fed") hasFed = true;
        if (action == "fetched") hasFetched = true;

        if (hasFed && hasFetched)
        {
            Debug.Log("Both actions completed. Loading next scene...");
            FindObjectOfType<SceneController>().LoadNextScene();
        }
    }
}
