using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeScript : MonoBehaviour
{
    [SerializeField] private Image fadeImage;

    private void Start()
    {
        StartCoroutine(FadeOut());
    }
    
    public void FadeAndLoad(string SceneName, float duration)
    {
        StartCoroutine(Fader(SceneName, duration));
    }

    IEnumerator Fader(string SceneName, float duration)
    {
        float t = 0;
        Color c = fadeImage.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = t / duration;
            fadeImage.color = c;
            yield return null;
        }
        
        SceneManager.LoadScene(SceneName);
    }

    IEnumerator FadeOut()
    {
        float t = 0;
        Color c = fadeImage.color;

        while (t < 1)
        {
            t += Time.deltaTime;
            c.a = 1f - (t / 1f);
            fadeImage.color = c;
            yield return null;
        }
    }
    
}