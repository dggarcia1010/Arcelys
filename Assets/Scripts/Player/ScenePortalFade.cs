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

    // ---------- NUEVO: sistema de spawn ----------
    [Header("Spawn en escena destino")]
    public string targetSpawnId;         // ID del punto de spawn en la escena destino
    public static string nextSpawnId;    // lo leerá el SpawnPoint en la escena cargada
    // ---------------------------------------------

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

        // Guardamos a qué spawn queremos ir en la escena destino
        nextSpawnId = targetSpawnId;

        // Aseguramos que existe un ScreenFader
        var fader = ScreenFader.Instance;
        if (fader == null)
        {
            if (faderPrefab != null)
            {
                fader = Instantiate(faderPrefab);
            }
            else
            {
                Debug.LogError("Portal: No existe ScreenFader ni faderPrefab asignado.");
                isLoading = false;
                col.enabled = true;
                return;
            }
        }

        fader.StartCoroutine(FadeLoadThenCleanup(fader));
    }

    IEnumerator FadeLoadThenCleanup(ScreenFader fader)
    {
        // Fade out
        yield return fader.FadeTo(1f, fadeOutDuration, true);

        // Cargar escena
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Single);
        while (!op.isDone) yield return null;

        // Fade in
        yield return fader.FadeTo(0f, fadeInDuration, true);

        // IMPORTANTE: ya NO destruimos el fader
        // Destroy(fader.gameObject);

        isLoading = false;
        if (col != null) col.enabled = true;  // el objeto se destruirá con la escena anterior igualmente
    }
}