using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ProximityShowText : MonoBehaviour
{
    [Header("Qué mostrar/ocultar (UI o mundo)")]
    public GameObject targetToShow;        // arrastra aquí HintPanel

    [Header("Detección")]
    public string playerTag = "Player";

    [Header("Aparición suave (opcional)")]
    public bool useCanvasGroupIfAvailable = true;
    public float fadeDuration = 0.25f;

    [Header("Estado inicial")]
    public bool startHidden = true;

    Collider2D col;
    CanvasGroup cg;
    int insideCount = 0;
    Coroutine fadeCo;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true;

        if (targetToShow != null && useCanvasGroupIfAvailable)
            cg = targetToShow.GetComponent<CanvasGroup>();

        if (startHidden) HideImmediate();
        else ShowImmediate();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        insideCount++;
        Show();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        insideCount = Mathf.Max(insideCount - 1, 0);
        if (insideCount == 0) Hide();
    }

    void OnDisable()
    {
        insideCount = 0;
        HideImmediate();
    }

    // ---- Mostrar / Ocultar ----
    void Show()
    {
        if (targetToShow == null) return;

        if (cg != null && useCanvasGroupIfAvailable)
        {
            targetToShow.SetActive(true);
            StartFade(1f);
        }
        else
        {
            targetToShow.SetActive(true);
        }
    }

    void Hide()
    {
        if (targetToShow == null) return;

        if (cg != null && useCanvasGroupIfAvailable)
        {
            StartFade(0f, whenDone: () =>
            {
                if (Mathf.Approximately(cg.alpha, 0f)) targetToShow.SetActive(false);
            });
        }
        else
        {
            targetToShow.SetActive(false);
        }
    }

    void ShowImmediate()
    {
        if (targetToShow == null) return;
        if (cg != null) { cg.alpha = 1f; cg.blocksRaycasts = true; }
        targetToShow.SetActive(true);
    }

    void HideImmediate()
    {
        if (targetToShow == null) return;
        if (cg != null) { cg.alpha = 0f; cg.blocksRaycasts = false; }
        targetToShow.SetActive(false);
    }

    void StartFade(float target, System.Action whenDone = null)
    {
        if (fadeCo != null) StopCoroutine(fadeCo);
        fadeCo = StartCoroutine(FadeTo(target, whenDone));
    }

    IEnumerator FadeTo(float target, System.Action whenDone)
    {
        float start = cg.alpha;
        float t = 0f;
        cg.blocksRaycasts = true;

        if (fadeDuration <= 0f)
        {
            cg.alpha = target;
            cg.blocksRaycasts = !Mathf.Approximately(target, 0f);
            whenDone?.Invoke();
            yield break;
        }

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime; // no se afecta por Time.timeScale
            cg.alpha = Mathf.Lerp(start, target, t / fadeDuration);
            yield return null;
        }
        cg.alpha = target;
        cg.blocksRaycasts = !Mathf.Approximately(target, 0f);
        whenDone?.Invoke();
    }
}