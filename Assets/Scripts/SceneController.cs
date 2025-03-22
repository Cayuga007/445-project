using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void LoadNextScene()
    {
        int current_day = int.Parse(SceneManager.GetActiveScene().name.Replace("day", ""));

        if (current_day < 4)
        {
            SceneManager.LoadScene("day" + (current_day + 1).ToString());
        }
        else
        {
            Debug.Log("End of experience");
        }
    }
}
