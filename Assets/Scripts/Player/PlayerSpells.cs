using UnityEngine;

public class PlayerSpells : MonoBehaviour
{

    [Header("Cooldowns (segundos)")]
    public float windCooldown = 2f;
    public float iceCooldown = 3f;
    public float fireCooldown = 4f;
    public float lightCooldown = 5f;

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
        windTimer -= Time.deltaTime;
        iceTimer -= Time.deltaTime;
        fireTimer -= Time.deltaTime;
        lightTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Alpha1)) CastSpell(windPrefab, ref windTimer, windCooldown, "VIENTO");
        if (Input.GetKeyDown(KeyCode.Alpha2)) CastSpell(icePrefab, ref iceTimer, iceCooldown, "HIELO");
        if (Input.GetKeyDown(KeyCode.Alpha3)) CastSpell(firePrefab, ref fireTimer, fireCooldown, "FUEGO");
        if (Input.GetKeyDown(KeyCode.Alpha4)) CastSpell(lightPrefab, ref lightTimer, lightCooldown, "LUZ");
    }

    void CastSpell(GameObject prefab, ref float timer, float cooldown, string spellName)
    {
        if (timer > 0 || prefab == null) return;

        timer = cooldown;
        Debug.Log($"Lanzaste {spellName}");

        float distanceToCamera = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceToCamera)
        );
        mouseWorldPos.z = transform.position.z;

        Vector2 direction = (mouseWorldPos - transform.position).normalized;

        GameObject proj = Instantiate(prefab, transform.position, Quaternion.identity);

        Projectile2D p = proj.GetComponent<Projectile2D>();
        if (p == null) p = proj.AddComponent<Projectile2D>();

        p.direction = direction;
        p.speed = projectileSpeed;

        p.isWind = (prefab == windPrefab);
        p.isFire = (prefab == firePrefab);
        p.isIce  = (prefab == icePrefab);

        if (prefab == windPrefab)
        {
            p.damage = 5; // utility spell
            p.windPushForce = 1f;
        }
        else if (prefab == icePrefab)
        {
            p.damage = 5;
            p.slowMultiplier = 0.4f;
            p.slowDuration   = 3f;
        }
        else if (prefab == firePrefab)
        {
            p.damage = 3;
        }
        else if (prefab == lightPrefab)
        {
            p.damage = 3;
        }
        else
        {
            p.damage = 1;
        }
            
    }

    public float GetCooldownRemaining(int spell)
    {
        switch (spell)
        {
            case 1: return Mathf.Max(0, windTimer);
            case 2: return Mathf.Max(0, iceTimer);
            case 3: return Mathf.Max(0, fireTimer);
            case 4: return Mathf.Max(0, lightTimer);
            default: return 0;
        }
    }
}