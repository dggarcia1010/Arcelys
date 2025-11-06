using UnityEngine;

public class PatrollingEnemy : Enemy
{
    [Header("Comportamiento Patrol")]
    public Transform[] patrolPoints; 
    public float tolerance = 0.5f; 

    private int currentPointIndex = 0;

    protected override void Start()
    {
        // No llamamos a base.Start() porque no necesitamos la referencia al jugador
        if (patrolPoints == null || patrolPoints.Length < 2) 
        {
            Debug.LogError("El enemigo patrullero necesita al menos 2 puntos de patrulla asignados.");
            enabled = false; 
            return;
        }
        
        // Inicialización del Rigidbody
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError($"[ID: {gameObject.name}] Rigidbody2D es necesario para el movimiento basado en fuerza. Desactivando script.");
            enabled = false;
            return;
        }
        rb.gravityScale = 0f;
        rb.freezeRotation = true; 

        // Coloca al enemigo en el primer punto al inicio
        transform.position = patrolPoints[0].position; 
    }

    protected override void Move()
    {
        if (patrolPoints == null || patrolPoints.Length < 2) return;

        Vector2 targetPosition2D = patrolPoints[currentPointIndex].position;
        Vector2 currentPosition2D = transform.position;

        // Mueve al enemigo usando MoveTowards (más preciso para puntos de destino)
        transform.position = Vector2.MoveTowards(
            currentPosition2D, 
            targetPosition2D,  
            speed * Time.fixedDeltaTime 
        );

        // Verifica si el enemigo ha llegado al punto objetivo
        if (Vector2.Distance(currentPosition2D, targetPosition2D) < tolerance)
        {
            // Cambia al siguiente punto en el array
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }
    }
}
