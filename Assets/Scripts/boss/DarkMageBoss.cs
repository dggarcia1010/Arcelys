using UnityEngine;
using System.Collections;

public class DarkMageBoss : Enemy
{
    [Header("Distancias")]
    public float aggroDistance = 12f;

    [Header("Cooldowns")]
    public float orbCooldown = 2.5f;
    public float lightningCooldown = 5f;

    [Header("Post-cast delay (global)")]
    public float postAbilityDelay = 2f;

    [Header("Prefabs")]
    public GameObject darkOrbPrefab;
    public GameObject lightningWarningPrefab;
    public GameObject lightningStrikePrefab;

    [Header("Movimiento (solo eje Y)")]
    public float yMoveSpeed = 2.5f;
    public float yChangeMinTime = 0.6f;
    public float yChangeMaxTime = 1.6f;

    [Header("Límites de movimiento en Y")]
    public float minY = -3f;
    public float maxY = 3f;

    [Header("Fase 2 (mitad de vida)")]
    [Tooltip("Ángulos (grados) del abanico de 5 disparos")]
    public float[] spreadAngles = new float[] { -20f, -10f, 0f, 10f, 20f };
    public float spreadOrbSpeed = 3f;

    private float nextOrbTime = 0f;
    private float nextLightningTime = 0f;
    private float nextActionTime = 0f;

    private bool doLightningNext = false;

    // Movimiento Y
    private float nextYChangeTime = 0f;
    private float currentYDir = 0f;

    // Fase 2
    private bool phase2Unlocked = false;
    private EnemyHealth healthRef;

    void Awake()
    {
        healthRef = GetComponent<EnemyHealth>();
    }

    protected override void Move()
    {
        if (playerTransform == null || rb == null) return;

        float dist = Vector2.Distance(transform.position, playerTransform.position);

        if (dist > aggroDistance)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        TryUnlockPhase2();

        if (!isAttacking)
        {
            UpdateRandomYMovement();
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (!isAttacking && Time.time >= nextActionTime)
        {
            if (doLightningNext)
            {
                if (Time.time >= nextLightningTime)
                    StartCoroutine(LightningAttack());
            }
            else
            {
                if (Time.time >= nextOrbTime)
                {
                    if (phase2Unlocked)
                        StartCoroutine(OrbSpreadAttack());
                    else
                        StartCoroutine(OrbAttack());
                }
            }
        }
    }

    void TryUnlockPhase2()
    {
        if (phase2Unlocked || healthRef == null) return;

        if (healthRef.currentHealth <= Mathf.CeilToInt(healthRef.maxHealth * 0.5f))
        {
            phase2Unlocked = true;
        }
    }

    void UpdateRandomYMovement()
    {
        if (Time.time >= nextYChangeTime)
        {
            currentYDir = Random.Range(-1f, 1f);

            if (Mathf.Abs(currentYDir) < 0.15f)
                currentYDir = Mathf.Sign(Random.Range(-1f, 1f));

            float t = Random.Range(yChangeMinTime, yChangeMaxTime);
            nextYChangeTime = Time.time + t;
        }

        float newY = rb.position.y + currentYDir * yMoveSpeed * Time.deltaTime;

        // 🔒 Clamp en Y
        newY = Mathf.Clamp(newY, minY, maxY);

        rb.MovePosition(new Vector2(rb.position.x, newY));
    }

    IEnumerator OrbAttack()
    {
        isAttacking = true;
        animator.SetTrigger("CastOrb");

        yield return new WaitForSeconds(0.35f);

        GetComponent<EnemySFX>()?.PlayAttackSFX();

        Vector2 dir = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;

        SpawnOrb(dir, spreadOrbSpeed);

        nextOrbTime = Time.time + orbCooldown;
        doLightningNext = true;

        yield return new WaitForSeconds(0.4f);

        isAttacking = false;
        nextActionTime = Time.time + postAbilityDelay;
    }

    IEnumerator OrbSpreadAttack()
    {
        isAttacking = true;
        animator.SetTrigger("CastOrb");

        yield return new WaitForSeconds(0.35f);

        GetComponent<EnemySFX>()?.PlayAttackSFX();

        Vector2 baseDir = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
        if (baseDir == Vector2.zero) baseDir = Vector2.down;

        foreach (float angle in spreadAngles)
        {
            Vector2 dir = Rotate(baseDir, angle);
            SpawnOrb(dir, spreadOrbSpeed);
        }

        nextOrbTime = Time.time + orbCooldown;
        doLightningNext = true;

        yield return new WaitForSeconds(0.4f);

        isAttacking = false;
        nextActionTime = Time.time + postAbilityDelay;
    }

    IEnumerator LightningAttack()
    {
        isAttacking = true;
        animator.SetTrigger("CastLightning");

        Vector3 targetPos = playerTransform.position;

        GameObject warning = Instantiate(
            lightningWarningPrefab,
            targetPos,
            Quaternion.identity
        );

        yield return new WaitForSeconds(1f);

        if (warning != null)
            Destroy(warning);

        GetComponent<EnemySFX>()?.PlayAttackSFX();

        Instantiate(
            lightningStrikePrefab,
            targetPos,
            Quaternion.identity
        );

        nextLightningTime = Time.time + lightningCooldown;
        doLightningNext = false;

        yield return new WaitForSeconds(0.3f);

        isAttacking = false;
        nextActionTime = Time.time + postAbilityDelay;
    }

    void SpawnOrb(Vector2 dir, float speed)
    {
        GameObject orb = Instantiate(darkOrbPrefab, transform.position, Quaternion.identity);

        Projectile2D p = orb.GetComponent<Projectile2D>();
        if (p == null) p = orb.AddComponent<Projectile2D>();

        p.direction = dir;
        p.speed = speed;
        p.damage = 2;
        p.lifeTime = 6f;
        p.team = Projectile2D.Team.Enemy;
        p.ownerCollider = GetComponent<Collider2D>();
    }

    Vector2 Rotate(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(rad);
        float cos = Mathf.Cos(rad);
        return new Vector2(
            v.x * cos - v.y * sin,
            v.x * sin + v.y * cos
        ).normalized;
    }
}