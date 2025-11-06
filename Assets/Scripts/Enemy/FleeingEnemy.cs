using UnityEngine;

public class FleeingEnemy : Enemy
{
    public float fleeDistance = 5.0f;    // Distancia a la que empieza a huir agresivamente.
    public float safeDistance = 6.0f;    // Distancia óptima que el enemigo intenta mantener.
    public float stopDistance = 8.0f;    // Distancia a la que se detiene y frena.

    protected override void Start()
    {
        base.Start();
    }

    protected override void Move()
    {
        if (playerTransform == null || rb == null) return;

        Vector2 targetPosition = playerTransform.position;
        Vector2 currentPosition = rb.position;
        float distanceToPlayer = Vector2.Distance(currentPosition, targetPosition);
        Vector2 desiredVelocity;

        // 1. Lógica de Huida Agresiva (Flee)
        if (distanceToPlayer < fleeDistance)
        {
            // Dirección opuesta al jugador.
            Vector2 direction = (currentPosition - targetPosition).normalized;
            desiredVelocity = direction * speed;

            // Calcular y limitar la fuerza de huida (Steering Force).
            Vector2 steeringForce = desiredVelocity - rb.linearVelocity;
            steeringForce = Vector2.ClampMagnitude(steeringForce, maxForce);
            
            rb.AddForce(steeringForce);
        }
        // 2. Lógica de Mantenimiento de Distancia Segura
        else if (distanceToPlayer >= fleeDistance && distanceToPlayer < stopDistance)
        {
            float approachFactor = 0.5f; 

            if (distanceToPlayer > safeDistance)
            {
                // Acercarse suavemente.
                Vector2 direction = (targetPosition - currentPosition).normalized;
                desiredVelocity = direction * (speed * approachFactor); 
            }
            else if (distanceToPlayer < safeDistance)
            {
                // Alejarse suavemente.
                Vector2 direction = (currentPosition - targetPosition).normalized;
                desiredVelocity = direction * (speed * approachFactor);
            }
            else // Está en la distancia segura, intentar detenerse.
            {
                desiredVelocity = Vector2.zero;
            }

            Vector2 steeringForce = desiredVelocity - rb.linearVelocity;
            steeringForce = Vector2.ClampMagnitude(steeringForce, maxForce);
            rb.AddForce(steeringForce);

            // Estabilización con fricción.
            if (rb.linearVelocity.sqrMagnitude > 0.01f)
            {
                rb.AddForce(-rb.linearVelocity * maxForce * 0.2f);
            }
        }
        // 3. Lógica de Detención Suave (Braking)
        else // distanceToPlayer >= stopDistance
        {
            if (rb.linearVelocity.sqrMagnitude > 0.01f)
            {
                // Aplicar fuerza de frenado.
                rb.AddForce(-rb.linearVelocity * maxForce * 0.8f); 
            }
            else
            {
                rb.linearVelocity = Vector2.zero; 
            }
        }
    }
}
