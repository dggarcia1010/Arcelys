using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BatSFX : MonoBehaviour
{
    [Header("Bat sounds")]
    [Tooltip("Chillidos / aleteos del murciélago")]
    public AudioClip[] batSounds;

    [Header("Timing (segundos)")]
    public float minTime = 2f;
    public float maxTime = 6f;

    [Header("Audio")]
    [Range(0f, 1f)] public float volume = 0.5f;
    public float pitchMin = 0.95f;
    public float pitchMax = 1.2f;

    private AudioSource audioSource;
    private Coroutine soundCoroutine;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 1f; // 3D
    }

    void Start()
    {
        if (batSounds != null && batSounds.Length > 0)
        {
            soundCoroutine = StartCoroutine(BatSoundRoutine());
        }
    }

    IEnumerator BatSoundRoutine()
    {
        while (true)
        {
            float wait = Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(wait);

            PlayBatSound();
        }
    }

    void PlayBatSound()
    {
        if (batSounds == null || batSounds.Length == 0) return;

        AudioClip clip = batSounds[Random.Range(0, batSounds.Length)];
        audioSource.pitch = Random.Range(pitchMin, pitchMax);
        audioSource.PlayOneShot(clip, volume);
    }

    // 🛑 Llamar al morir
    public void StopAllBatSounds()
    {
        if (soundCoroutine != null)
        {
            StopCoroutine(soundCoroutine);
            soundCoroutine = null;
        }

        audioSource.Stop();
    }

    void OnDisable()
    {
        StopAllBatSounds();
    }
}
