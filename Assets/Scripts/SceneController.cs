using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    public Image fadeImage; // Assign a UI Image in the Inspector
    private float blackFadeDuration = 2f;
    private float whiteFadeDuration = 10f;
    private int current_day = 0;

    private void Start()
    {
        current_day = int.Parse(SceneManager.GetActiveScene().name.Replace("day", ""));
        FadeIn();
    }

    public void LoadNextScene()
    {
        if (current_day < 4)
        {
            // Fade to black for days 1–3 and load the next scene
            StartCoroutine(FadeAndLoadScene("day" + (current_day + 1).ToString(), Color.black, blackFadeDuration));
        }
        else
        {
            // Fade to white on day 4 before quitting
            StartCoroutine(FadeAndQuit(Color.white, whiteFadeDuration));
        }
    }

    private void FadeIn()
    {
        fadeImage.gameObject.SetActive(true);
        fadeImage.color = new Color(0, 0, 0, 1); // Start fully black
        LeanTween.value(fadeImage.gameObject, 1f, 0f, blackFadeDuration)
            .setOnUpdate((float val) => fadeImage.color = new Color(0, 0, 0, val))
            .setEase(LeanTweenType.easeInOutQuad);
    }

    private IEnumerator FadeAndLoadScene(string sceneName, Color fadeColor, float duration)
    {
        // Wait for fade out to complete
        yield return FadeOut(fadeColor, duration);
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeAndQuit(Color fadeColor, float duration)
    {
        // Wait for fade out to complete
        yield return FadeOut(fadeColor, duration);
        Application.Quit();
    }

    private IEnumerator FadeOut(Color fadeColor, float duration)
    {
        bool fadeComplete = false;
        fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0); // Ensure the correct starting color
        LeanTween.value(fadeImage.gameObject, 0f, 1f, duration)
            .setOnUpdate((float val) => fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, val))
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() => fadeComplete = true);

        yield return new WaitUntil(() => fadeComplete);
    }
}
