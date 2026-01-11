using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RatSFX : MonoBehaviour
{
    [Header("Rat sounds")]
    [Tooltip("Chillidos / sonidos de la rata")]
    public AudioClip[] squeaks;

    [Header("Timing (segundos)")]
    [Tooltip("Tiempo mínimo entre sonidos")]
    public float minTime = 3f;

    [Tooltip("Tiempo máximo entre sonidos")]
    public float maxTime = 8f;

    [Header("Audio")]
    [Range(0f, 1f)] public float volume = 0.6f;
    public float pitchMin = 0.9f;
    public float pitchMax = 1.1f;

    private AudioSource audioSource;
    private Coroutine squeakCoroutine;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // Ajustes recomendados
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 1f; // 3D para distancia
    }

    void Start()
    {
        // Solo empezamos si hay sonidos asignados
        if (squeaks != null && squeaks.Length > 0)
        {
            squeakCoroutine = StartCoroutine(SqueakRoutine());
        }
    }

    IEnumerator SqueakRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(waitTime);

            PlaySqueak();
        }
    }

    void PlaySqueak()
    {
        if (squeaks == null || squeaks.Length == 0) return;

        AudioClip clip = squeaks[Random.Range(0, squeaks.Length)];
        audioSource.pitch = Random.Range(pitchMin, pitchMax);
        audioSource.PlayOneShot(clip, volume);
    }

    // ✅ Llama a esto cuando la rata muera
    public void StopAllRatSounds()
    {
        if (squeakCoroutine != null)
        {
            StopCoroutine(squeakCoroutine);
            squeakCoroutine = null;
        }

        audioSource.Stop();
    }

    // Seguridad extra: si el objeto se desactiva o destruye
    void OnDisable()
    {
        StopAllRatSounds();
    }
}

