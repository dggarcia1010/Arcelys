using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FairyCooldownGlow : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerSpells playerSpells;
    [SerializeField] private Light2D auraLight;

    [Header("Colores por hechizo")]
    public Color windColor = new Color(0.3f, 1f, 0.6f);
    public Color iceColor  = new Color(0.3f, 0.7f, 1f);
    public Color fireColor = new Color(1f, 0.35f, 0.2f);

    [Header("Intensidad")]
    public float maxIntensity = 1.2f; 
    public float minIntensity = 0.0f;   
    public float smoothSpeed = 10f;    

    private float currentIntensity;

    void Awake()
    {
        if (!auraLight)
            auraLight = GetComponentInChildren<Light2D>(true);

        if (!playerSpells)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) playerSpells = p.GetComponent<PlayerSpells>();
        }

        if (auraLight != null)
            currentIntensity = auraLight.intensity;
    }

    void Update()
    {
        if (!playerSpells || !auraLight) return;

        var spell = playerSpells.CurrentSpell;

        if (spell != PlayerSpells.SpellType.Wind &&
            spell != PlayerSpells.SpellType.Ice &&
            spell != PlayerSpells.SpellType.Fire)
        {
            auraLight.intensity = 0f; 
            return;
        }

        auraLight.color = GetColor(spell);

        float total = GetTotalCooldown(spell);
        float remaining = playerSpells.GetCooldownRemaining(spell);

        float t = 1f;
        if (total > 0.0001f)
        {

            t = 1f - Mathf.Clamp01(remaining / total);
        }

        float targetIntensity = Mathf.Lerp(minIntensity, maxIntensity, t);

        currentIntensity = Mathf.Lerp(currentIntensity, targetIntensity, smoothSpeed * Time.deltaTime);
        auraLight.intensity = currentIntensity;
    }

    Color GetColor(PlayerSpells.SpellType spell)
    {
        switch (spell)
        {
            case PlayerSpells.SpellType.Wind: return windColor;
            case PlayerSpells.SpellType.Ice:  return iceColor;
            case PlayerSpells.SpellType.Fire: return fireColor;
            default: return Color.white;
        }
    }

    float GetTotalCooldown(PlayerSpells.SpellType spell)
    {
        switch (spell)
        {
            case PlayerSpells.SpellType.Wind: return playerSpells.windCooldown;
            case PlayerSpells.SpellType.Ice:  return playerSpells.iceCooldown;
            case PlayerSpells.SpellType.Fire: return playerSpells.fireCooldown;
            default: return 0f;
        }
    }
}