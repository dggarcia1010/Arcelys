using UnityEngine;

public class PlayerSpells : MonoBehaviour
{
    public enum SpellType { None, Wind, Ice, Fire, Light }

    [Header("Selección actual (solo lectura)")]
    [SerializeField] private SpellType currentSpell = SpellType.None;

    [Header("Estado")]
    [SerializeField] private bool magicUnlocked = false;
    public bool MagicUnlocked => magicUnlocked;

    [Header("Cooldowns (segundos)")]
    public float windCooldown = 2f;
    public float iceCooldown  = 3f;
    public float fireCooldown = 4f;
    public float lightCooldown= 5f;

    private float windTimer;
    private float iceTimer;
    private float fireTimer;
    private float lightTimer;

    [Header("Proyectiles (asignar prefabs)")]
    public GameObject windPrefab;
    public GameObject icePrefab;
    public GameObject firePrefab;
    public GameObject lightPrefab;

    [Header("Velocidad proyectiles")]
    public float projectileSpeed = 5f;

    void Update()
    {
        if (windTimer  > 0) windTimer  -= Time.deltaTime;
        if (iceTimer   > 0) iceTimer   -= Time.deltaTime;
        if (fireTimer  > 0) fireTimer  -= Time.deltaTime;
        if (lightTimer > 0) lightTimer -= Time.deltaTime;

        if (!magicUnlocked) return;

        // Seleccionar (1–4)
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectSpell(SpellType.Wind);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectSpell(SpellType.Ice);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectSpell(SpellType.Fire);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectSpell(SpellType.Light);

        // Lanzar (Espacio)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CastSelectedSpell();
        }
    }

    public void UnlockMagic()
    {
        if (magicUnlocked) return;
        magicUnlocked = true;
        Debug.Log("🪄 ¡Magia habilitada! Usa 1–4 para elegir y Espacio para lanzar.");
    }

    void SelectSpell(SpellType spell)
    {
        currentSpell = spell;
        Debug.Log($"Hechizo seleccionado: {currentSpell}");
    }

    void CastSelectedSpell()
    {
        if (currentSpell == SpellType.None)
        {
            Debug.Log("No hay hechizo seleccionado. Usa 1-4 para seleccionar.");
            return;
        }

        GameObject prefab = null;
        float cooldown = 0f;
        ref float timer = ref windTimer;

        switch (currentSpell)
        {
            case SpellType.Wind:
                prefab = windPrefab; cooldown = windCooldown; timer = ref windTimer; break;
            case SpellType.Ice:
                prefab = icePrefab;  cooldown = iceCooldown;  timer = ref iceTimer;  break;
            case SpellType.Fire:
                prefab = firePrefab; cooldown = fireCooldown; timer = ref fireTimer; break;
            case SpellType.Light:
                prefab = lightPrefab;cooldown = lightCooldown;timer = ref lightTimer;break;
        }

        if (prefab == null)
        {
            Debug.LogWarning($"Prefab no asignado para {currentSpell}.");
            return;
        }

        if (timer > 0f)
        {
            Debug.Log($"[{currentSpell}] en cooldown: {timer:0.00}s");
            return;
        }

        timer = cooldown;
        Debug.Log($"Lanzaste {currentSpell}");

        float distToCam = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, distToCam)
        );
        mouseWorldPos.z = transform.position.z;

        Vector2 direction = (mouseWorldPos - transform.position).normalized;

        GameObject proj = Instantiate(prefab, transform.position, Quaternion.identity);

        Projectile2D p = proj.GetComponent<Projectile2D>();
        if (p == null) p = proj.AddComponent<Projectile2D>();

        p.direction = direction;
        p.speed = projectileSpeed;

        p.isWind = (currentSpell == SpellType.Wind);
        p.isIce  = (currentSpell == SpellType.Ice);
        p.isFire = (currentSpell == SpellType.Fire);

        if (currentSpell == SpellType.Wind)
        {
            p.damage = 1;
            p.windPushForce = 2f;
        }
        else if (currentSpell == SpellType.Ice)
        {
            p.damage = 1;
            p.slowMultiplier = 0.4f;
            p.slowDuration   = 3f;
        }
        else if (currentSpell == SpellType.Fire)
        {
            p.damage = 2;
        }
        else if (currentSpell == SpellType.Light)
        {
            p.damage = 3;
        }
        else
        {
            p.damage = 1;
        }
    }

    // PARA FUTURO UI
    public float GetCooldownRemaining(SpellType spell)
    {
        switch (spell)
        {
            case SpellType.Wind: return Mathf.Max(0, windTimer);
            case SpellType.Ice: return Mathf.Max(0, iceTimer);
            case SpellType.Fire: return Mathf.Max(0, fireTimer);
            case SpellType.Light: return Mathf.Max(0, lightTimer);
            default: return 0;
        }
    }
    
    //PARA FUTURO HUD
    public SpellType CurrentSpell => currentSpell;
}