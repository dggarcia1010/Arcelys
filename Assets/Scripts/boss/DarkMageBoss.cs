using UnityEngine;
using System.Collections;

public class DarkMageBoss : Enemy
{
    [Header("Distancias")]
    public float aggroDistance = 12f;
    public float keepDistance = 6f;

    [Header("Cooldowns")]
    public float orbCooldown = 2.5f;
    public float lightningCooldown = 5f;

    [Header("Prefabs")]
    public GameObject darkOrbPrefab;
    public GameObject lightningWarningPrefab;
    public GameObject lightningStrikePrefab;

    private float nextOrbTime = 0f;
    private float nextLightningTime = 0f;
    private bool doLightningNext = false;

    protected override void Move()
    {
        if (playerTransform == null || rb == null) return;

        float dist = Vector2.Distance(transform.position, playerTransform.position);

        // Si está lejos, no hace nada
        if (dist > aggroDistance)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Mantener distancia (flotar lejos si el jugador se acerca demasiado)
        if (!isAttacking && dist < keepDistance)
        {
            Vector2 away = ((Vector2)transform.position - (Vector2)playerTransform.position).normalized;
            rb.linearVelocity = away * speed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Ataques alternados
        if (!isAttacking)
        {
            if (doLightningNext && Time.time >= nextLightningTime)
                StartCoroutine(LightningAttack());
            else if (!doLightningNext && Time.time >= nextOrbTime)
                StartCoroutine(OrbAttack());
        }
    }

    IEnumerator OrbAttack()
    {
        isAttacking = true;
        animator.SetTrigger("CastOrb");

        // pequeño delay para que cuadre con la animación
        yield return new WaitForSeconds(0.35f);

        Vector2 dir = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;

        GameObject orb = Instantiate(darkOrbPrefab, transform.position, Quaternion.identity);

        Projectile2D p = orb.GetComponent<Projectile2D>();
        if (p == null) p = orb.AddComponent<Projectile2D>();

        p.direction = dir;
        p.speed = 3f;     // bola lenta
        p.damage = 2;
        p.lifeTime = 6f;    
        p.team = Projectile2D.Team.Enemy;
        p.ownerCollider = GetComponent<Collider2D>();

        nextOrbTime = Time.time + orbCooldown;
        doLightningNext = true;

        yield return new WaitForSeconds(0.4f);
        isAttacking = false;
    }

    IEnumerator LightningAttack()
    {
        isAttacking = true;
        animator.SetTrigger("CastLightning");

        Vector3 targetPos = playerTransform.position;

        // 1️⃣ Aparece el warning
        GameObject warning = Instantiate(
            lightningWarningPrefab,
            targetPos,
            Quaternion.identity
        );

        // 2️⃣ Espera EXACTAMENTE 1 segundo
        yield return new WaitForSeconds(1f);

        // 3️⃣ El warning desaparece
        if (warning != null)
            Destroy(warning);

        // 4️⃣ Cae el rayo
        Instantiate(
            lightningStrikePrefab,
            targetPos,
            Quaternion.identity
        );

        // 5️⃣ Cooldown y reset
        nextLightningTime = Time.time + lightningCooldown;
        doLightningNext = false;

        // pequeño buffer para que la anim termine
        yield return new WaitForSeconds(0.3f);

        isAttacking = false;
    }
}