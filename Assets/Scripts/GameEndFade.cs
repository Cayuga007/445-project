using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameEndFade : MonoBehaviour
{
    public Image fadeImage; // Assign the UI Image in the Inspector
    public float fadeDuration = 10f; // Time for the fade effect
    public float waitBeforeFade = 10f; // Time before fade starts

    public void StartGameEndSequence()
    {
        StartCoroutine(FadeToWhite());
    }

    private IEnumerator FadeToWhite()
    {
        yield return new WaitForSeconds(waitBeforeFade); // Wait before fading

        float elapsedTime = 0f;
        Color startColor = fadeImage.color;
        startColor.a = 0f;
        fadeImage.color = startColor;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        QuitGame(); // Call the function to quit
    }

    private void QuitGame()
    {
        Debug.Log("Game Ended!");
        Application.Quit(); // Quits the game (works in build)

        // If running in editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
