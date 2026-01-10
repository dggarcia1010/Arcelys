using UnityEngine;

public class EnemySFX : MonoBehaviour
{
    [Header("Attack sounds")]
    public AudioClip[] attackClips;

    [Range(0f, 1f)] public float volume = 0.8f;
    public float pitchMin = 0.95f;
    public float pitchMax = 1.05f;

    private AudioSource a;

    void Awake()
    {
        a = GetComponent<AudioSource>();
        if (a == null) a = gameObject.AddComponent<AudioSource>();

        a.playOnAwake = false;
        a.loop = false;
    }

    // Llama a esto cuando el enemigo ataque
    public void PlayAttackSFX()
    {
        if (attackClips == null || attackClips.Length == 0) return;

        var clip = attackClips[Random.Range(0, attackClips.Length)];
        a.pitch = Random.Range(pitchMin, pitchMax);
        a.PlayOneShot(clip, volume);
    }
}

