using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public Image black;
    public Image white;
    private float black_fade_duration = 2f;
    private float white_fade_duration = 10f;
    private int current_day;

    private void Start()
    {
        current_day = int.Parse(SceneManager.GetActiveScene().name.Replace("day", ""));
        LeanTween.alpha(black.gameObject, 0, black_fade_duration);
    }
    public void LoadNextScene()
    {
        if (current_day < 4)
        {
            LeanTween.alpha(black.gameObject, 1, black_fade_duration).setOnComplete(NextDay);
        } else
        {
            LeanTween.alpha(white.gameObject, 1, white_fade_duration).setOnComplete(NextDay);
        }
    }

    public void NextDay()
    {
        SceneManager.LoadScene("day" + (current_day + 1).ToString());
    }

    public void LastDay()
    {
        Application.Quit();
    }
}
