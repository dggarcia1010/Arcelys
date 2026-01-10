using UnityEngine;

public class FollowerEnemy : Enemy
{
    [Header("Configuración de Seguimiento")]
    public Transform targetToFollow; // El enemigo al que sigue al principio
    public float stoppingDistance = 1.8f;

    // ── NUEVOS CAMPOS PARA LA ÓRBITA MÁGICA (mínimo añadido) ──
    [Header("Órbita mágica en puerta")]
    public float orbitRadius = 1.4f;
    public float orbitSpeed = 120f;       // grados por segundo (ajusta para velocidad del "hada")
    public Transform doorCenter;          // Se asignará cuando interactúes con la puerta

    private bool followingPlayer = false;
    private bool isOrbitingDoor = false;  // ← Nuevo estado
    private float orbitAngle = 0f;

    protected override void Start()
    {
        base.Start();
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
        }
    }

    protected override void Move()
    {
        if (rb == null) return;

        // Prioridad: si está en modo órbita mágica → movimiento circular perfecto
        if (isOrbitingDoor && doorCenter != null)
        {
            orbitAngle += orbitSpeed * Time.deltaTime;
            float rad = orbitAngle * Mathf.Deg2Rad;

            Vector2 orbitPosition = (Vector2)doorCenter.position + new Vector2(
                Mathf.Cos(rad) * orbitRadius,
                Mathf.Sin(rad) * orbitRadius
            );

            // Movimiento directo (sin física) para que sea suave como un hada
            rb.position = orbitPosition;
            rb.linearVelocity = Vector2.zero; // Detenemos cualquier velocidad residual

            // Importante: NO rotamos el sprite (como pediste)
            // Si quieres que mire hacia el centro, descomenta:
            // Vector2 dir = (Vector2)doorCenter.position - rb.position;
            // float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            // transform.rotation = Quaternion.Euler(0, 0, angle);

            return; // ← Salimos, no ejecutamos el seguimiento normal
        }

        // Lógica original de seguimiento (sin cambios)
        if (targetToFollow == null || !targetToFollow.gameObject.activeInHierarchy)
        {
            targetToFollow = playerTransform;
            followingPlayer = true;
        }

        if (targetToFollow == null) return;

        Vector2 targetPos = targetToFollow.position;
        Vector2 currentPos = rb.position;
        float distance = Vector2.Distance(currentPos, targetPos);

        if (distance > stoppingDistance)
        {
            Vector2 direction = (targetPos - currentPos).normalized;
            Vector2 desiredVelocity = direction * speed;
            Vector2 steering = desiredVelocity - rb.linearVelocity;
            steering = Vector2.ClampMagnitude(steering, maxForce);
            rb.AddForce(steering);
        }
        else
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, Time.deltaTime * 5f);
        }
    }

    public bool IsFollowingPlayer() => followingPlayer;

    // Nuevo método público que llamaremos desde la puerta
    public void StartMagicOrbit(Transform center)
    {
        doorCenter = center;
        isOrbitingDoor = true;
        
    }
}
