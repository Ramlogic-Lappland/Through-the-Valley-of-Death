using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeScript : MonoBehaviour
{
    public static FadeScript Instance;

    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;

    private Coroutine _currentFade;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SetAlpha(1f);
        StartFade(0f);
    }

    public IEnumerator FadeOut()
    {
        yield return FadeTo(1f);
    }

    public IEnumerator FadeIn()
    {
        yield return FadeTo(0f);
    }

    private void StartFade(float targetAlpha)
    {
        if (_currentFade != null)
            StopCoroutine(_currentFade);

        _currentFade = StartCoroutine(FadeTo(targetAlpha));
    }

    private IEnumerator FadeTo(float targetAlpha)
    {
        float startAlpha = fadeImage.color.a;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;

            float alpha = Mathf.Lerp
            (
                startAlpha,
                targetAlpha,
                timer / fadeDuration
            );

            SetAlpha(alpha);

            yield return null;
        }

        SetAlpha(targetAlpha);
        _currentFade = null;
    }

    private void SetAlpha(float alpha)
    {
        fadeImage.color = new Color(0f, 0f, 0f, alpha);
    }
}