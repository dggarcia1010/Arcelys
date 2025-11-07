using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class ScenePortalFade : MonoBehaviour
{
    [Header("Escena destino (nombre exacto en Assets/Scenes)")]
    public string sceneToLoad;

    [Header("Detección")]
    public string playerTag = "Player";

    [Header("Fade")]
    public ScreenFader faderPrefab;
    public float fadeOutDuration = 0.4f;
    public float fadeInDuration  = 0.4f;

    bool isLoading = false;
    Collider2D col;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isLoading) return;
        if (!other.CompareTag(playerTag)) return;

        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogWarning("Portal: falta sceneToLoad.");
            return;
        }

        isLoading = true;
        col.enabled = false;

        Time.timeScale = 1f;

        var fader = ScreenFader.Instance;
        if (fader == null)
        {
            Debug.LogError("Portal: No existe ScreenFader en escena. Coloca uno en la escena inicial.");
            isLoading = false;
            col.enabled = true;
            return;
        }

        fader.StartCoroutine(FadeLoadThenCleanup(fader));
    }

    IEnumerator FadeLoadThenCleanup(ScreenFader fader)
    {
        yield return fader.FadeTo(1f, fadeOutDuration, true);
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Single);
        while (!op.isDone) yield return null;

        yield return fader.FadeTo(0f, fadeInDuration, true);

        Destroy(fader.gameObject);

        isLoading = false;
        if (col != null) col.enabled = true;
    }
}