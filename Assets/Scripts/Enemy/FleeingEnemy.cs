using UnityEngine;

public class FleeingEnemy : Enemy
{
    public float fleeDistance = 5.0f;     // Distancia a la que empieza a huir agresivamente
    public float safeDistance = 6.0f;     // Distancia óptima que intenta mantener
    public float stopDistance = 8.0f;     // Distancia a la que se detiene

    // Parámetros para evitar quedarse pegado a paredes
    public float avoidanceRadius = 1.5f;   // Radio de detección de obstáculos
    public LayerMask obstacleLayer;       // Layer de paredes y obstáculos

    protected override void Start()
    {
        base.Start();
        if (obstacleLayer.value == 0) 
            obstacleLayer = LayerMask.GetMask("Default");
    }

    protected override void Move()
    {
        if (playerTransform == null || rb == null) return;

        Vector2 currentPosition = rb.position;
        Vector2 playerPosition = playerTransform.position;
        Vector2 toPlayer = playerPosition - currentPosition;  // Vector del enemigo al jugador
        float distanceToPlayer = toPlayer.magnitude;

        Vector2 totalForce = Vector2.zero;

        // 1. Huida agresiva
        if (distanceToPlayer < fleeDistance)
        {
            Vector2 fleeDirection = -toPlayer.normalized;

            // Evitación de obstáculos
            Vector2 avoidanceDirection = CalculateObstacleAvoidance(toPlayer, currentPosition);
            if (avoidanceDirection != Vector2.zero)
            {
                // Combinamos huida con evasión lateral (más peso a la evasión si hay obstáculo cerca)
                fleeDirection = (fleeDirection + avoidanceDirection * 2f).normalized;
            }

            Vector2 desiredVelocity = fleeDirection * speed;
            Vector2 steering = desiredVelocity - rb.linearVelocity;
            steering = Vector2.ClampMagnitude(steering, maxForce);

            totalForce += steering;
        }
        // 2. Mantener distancia segura
        else if (distanceToPlayer < stopDistance)
        {
            float approachFactor = 0.5f;
            Vector2 desiredVelocity;

            if (distanceToPlayer > safeDistance + 0.5f)
            {
                // Acercarse suavemente
                desiredVelocity = toPlayer.normalized * speed * approachFactor;
            }
            else if (distanceToPlayer < safeDistance - 0.5f)
            {
                // Alejarse suavemente
                desiredVelocity = -toPlayer.normalized * speed * approachFactor;
            }
            else
            {
                desiredVelocity = Vector2.zero;
            }

            Vector2 steering = desiredVelocity - rb.linearVelocity;
            steering = Vector2.ClampMagnitude(steering, maxForce);
            totalForce += steering;

            // Fricción para estabilizarse
            if (rb.linearVelocity.sqrMagnitude > 0.01f)
            {
                totalForce += -rb.linearVelocity * maxForce * 0.2f;
            }
        }
        // 3. Fuera del rango: frenado
        else
        {
            if (rb.linearVelocity.sqrMagnitude > 0.01f)
            {
                totalForce += -rb.linearVelocity * maxForce * 0.8f;
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
            }
        }

        // Aplicar fuerza
        if (totalForce != Vector2.zero)
        {
            rb.AddForce(totalForce);
        }
    }

    private Vector2 CalculateObstacleAvoidance(Vector2 toPlayer, Vector2 currentPosition)
    {
        // Dirección hacia adelante: si tiene velocidad usamos esa, si no la dirección de huida
        Vector2 forward = rb.linearVelocity.magnitude > 0.1f 
            ? rb.linearVelocity.normalized 
            : -toPlayer.normalized;

        // Tres rayos en forma de bigote: centro, +45° y -45°
        Vector2[] directions = new Vector2[3];
        directions[0] = forward;
        directions[1] = Quaternion.Euler(0, 0, 45) * forward;
        directions[2] = Quaternion.Euler(0, 0, -45) * forward;

        float longestDistance = 0f;
        Vector2 bestDirection = Vector2.zero;

        foreach (Vector2 dir in directions)
        {
            RaycastHit2D hit = Physics2D.Raycast(currentPosition, dir, avoidanceRadius, obstacleLayer);
            if (!hit.collider)  // No hay obstáculo en esta dirección
            {
                float distance = avoidanceRadius; // Consideramos todo el radio como "libre"
                if (distance > longestDistance)
                {
                    longestDistance = distance;
                    bestDirection = dir;
                }
            }
        }

        if (bestDirection != Vector2.zero)
        {
            return bestDirection.normalized;
        }

        // Si todo está bloqueado, al menos intentamos ir perpendicular al jugador
        Vector2 perpendicular = Vector2.Perpendicular(toPlayer).normalized;
        // Elegimos la dirección perpendicular que esté más libre
        RaycastHit2D hitLeft = Physics2D.Raycast(currentPosition, perpendicular, avoidanceRadius, obstacleLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(currentPosition, -perpendicular, avoidanceRadius, obstacleLayer);

        return !hitLeft.collider ? perpendicular : -perpendicular;
    }
}
