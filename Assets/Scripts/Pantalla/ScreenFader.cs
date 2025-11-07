using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance { get; private set; }
    CanvasGroup cg;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        cg = GetComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.blocksRaycasts = false;
        cg.interactable   = false;

        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator FadeTo(float targetAlpha, float duration, bool unscaled = true)
    {
        float start = cg.alpha;
        float t = 0f;
        cg.blocksRaycasts = true;

        if (duration <= 0f)
        {
            cg.alpha = targetAlpha;
            cg.blocksRaycasts = !Mathf.Approximately(targetAlpha, 0f);
            yield break;
        }

        while (t < duration)
        {
            t += unscaled ? Time.unscaledDeltaTime : Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, targetAlpha, t / duration);
            yield return null;
        }

        cg.alpha = targetAlpha;
        if (Mathf.Approximately(targetAlpha, 0f))
            cg.blocksRaycasts = false;
    }
}