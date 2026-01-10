using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerSpellSFX : MonoBehaviour
{
    [Header("Clips por hechizo")]
    public AudioClip wind;
    public AudioClip ice;
    public AudioClip fire;
    public AudioClip light;

    [Header("Ajustes")]
    [Range(0f, 1f)] public float volume = 0.8f;
    public float pitchMin = 0.95f;
    public float pitchMax = 1.05f;

    private AudioSource a;

    void Awake()
    {
        a = GetComponent<AudioSource>();

        // Ajustes recomendados para SFX
        a.playOnAwake = false;
        a.loop = false;

        // En top-down suele ir bien 2D.
        // Si prefieres que se atenúe por distancia, pon 1f.
        a.spatialBlend = 0f;
    }

    // ✅ Este es el método que vas a llamar desde PlayerSpells
    public void PlaySpell(PlayerSpells.SpellType spell)
    {
        AudioClip clip = null;

        switch (spell)
        {
            case PlayerSpells.SpellType.Wind:  clip = wind;  break;
            case PlayerSpells.SpellType.Ice:   clip = ice;   break;
            case PlayerSpells.SpellType.Fire:  clip = fire;  break;
            case PlayerSpells.SpellType.Light: clip = light; break;
            default: return; // None u otros
        }

        if (clip == null) return;

        a.pitch = Random.Range(pitchMin, pitchMax);
        a.PlayOneShot(clip, volume);
    }
}
