using UnityEngine;
using System.Collections;

public class BossGolem : Enemy
{
    [Header("IA Golem")]
    public float aggroDistance = 12f;
    public float slowdownRadius = 3f;
    public float stopDistance = 1.5f;

    [Header("Ataque de golpeo")]
    public float slamRange = 2.5f;
    public float slamRadius = 3f;
    public float slamDamage = 2f;
    public float attackTotalDuration = 0.8f;  // duración TOTAL de la anim de ataque
    public float slamCooldown = 2.5f;

    [Header("FX")]
    public GameObject slamWavePrefab;
    public LayerMask playerLayer;

    private float nextSlamTime = 0f;
    private Vector3 originalScale;

    protected override void Start()
    {
        base.Start();
        originalScale = transform.localScale;
    }

    protected override void Move()
    {
        if (playerTransform == null || rb == null) return;

        Vector2 playerPos = playerTransform.position;
        Vector2 currentPos = rb.position;
        float distance = Vector2.Distance(currentPos, playerPos);

        // Si el jugador está muy lejos, nos quedamos quietos
        if (distance > aggroDistance)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // ¿Podemos atacar?
        if (!isAttacking && distance <= slamRange && Time.time >= nextSlamTime)
        {
            rb.linearVelocity = Vector2.zero;
            StartCoroutine(SlamAttackCo());
            return;
        }

        if (isAttacking) return;

        // --- Persecución suave ---
        Vector2 desiredVelocity;

        if (distance > stopDistance)
        {
            Vector2 dir = (playerPos - currentPos).normalized;
            float targetSpeed = speed;

            if (distance < slowdownRadius)
                targetSpeed = speed * (distance / slowdownRadius);

            desiredVelocity = dir * targetSpeed;
        }
        else
        {
            desiredVelocity = Vector2.zero;
        }

        Vector2 steeringForce = desiredVelocity - rb.linearVelocity;
        steeringForce = Vector2.ClampMagnitude(steeringForce, maxForce);
        rb.AddForce(steeringForce);
    }

    private IEnumerator SlamAttackCo()
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;

        // mirar hacia el jugador
        if (playerTransform != null)
        {
            Vector2 dir = (playerTransform.position - transform.position);
            if (dir.sqrMagnitude > 0.001f)
                currentDirection = dir.normalized;
        }

        // Esperar a que termine la animación de ataque
        yield return new WaitForSeconds(attackTotalDuration);

        isAttacking = false;
        nextSlamTime = Time.time + slamCooldown;
    }

    // 👇 Esta función la llamará la ANIMACIÓN (Animation Event)
    public void OnSlamHit()
    {
        DoSlamDamage();

        if (slamWavePrefab != null)
            Instantiate(slamWavePrefab, transform.position, Quaternion.identity);
    }

    private void DoSlamDamage()
    {
        Collider2D[] hits;

        if (playerLayer.value != 0)
            hits = Physics2D.OverlapCircleAll(transform.position, slamRadius, playerLayer);
        else
            hits = Physics2D.OverlapCircleAll(transform.position, slamRadius);

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Player")) continue;

            var hs = hit.GetComponent<HealthSystem>();
            if (hs != null)
                hs.TakeDamage(slamDamage);
        }
    }

    void LateUpdate()
    {
        // Flip izquierda/derecha sin cambiar tamaño
        if (currentDirection.x > 0.1f)
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        else if (currentDirection.x < -0.1f)
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, slamRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, slamRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, aggroDistance);
    }
#endif
}