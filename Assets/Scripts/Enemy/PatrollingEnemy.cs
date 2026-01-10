using UnityEngine;

public class PatrollingEnemy : Enemy
{
    [Header("Comportamiento Patrol")]
    public Transform[] patrolPoints;
    public float tolerance = 0.5f;

    private int currentPointIndex = 0;

    // 🔄 Sprite para hacer flip
    private SpriteRenderer spriteRenderer;

    protected override void Start()
    {
        // No llamamos a base.Start() porque no necesitamos la referencia al jugador
        if (patrolPoints == null || patrolPoints.Length < 2)
        {
            Debug.LogError("El enemigo patrullero necesita al menos 2 puntos de patrulla asignados.");
            enabled = false;
            return;
        }

        // Rigidbody
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError($"[ID: {gameObject.name}] Rigidbody2D es necesario para el movimiento basado en fuerza. Desactivando script.");
            enabled = false;
            return;
        }
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        // SpriteRenderer (en este objeto o en hijos)
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // Posición inicial
        transform.position = patrolPoints[0].position;
    }

    protected override void Move()
    {
        if (patrolPoints == null || patrolPoints.Length < 2) return;

        Vector2 currentPosition2D = transform.position;
        Vector2 targetPosition2D = patrolPoints[currentPointIndex].position;

        // Dirección horizontal
        float dirX = targetPosition2D.x - currentPosition2D.x;

        // 🔄 Flip del sprite según la dirección
        if (spriteRenderer != null)
        {
            if (dirX < 0f)
                spriteRenderer.flipX = true;   // mira a la izquierda
            else if (dirX > 0f)
                spriteRenderer.flipX = false;  // mira a la derecha
        }

        // Movimiento hacia el punto
        transform.position = Vector2.MoveTowards(
            currentPosition2D,
            targetPosition2D,
            speed * Time.fixedDeltaTime
        );

        // ¿Ha llegado al punto?
        if (Vector2.Distance(currentPosition2D, targetPosition2D) < tolerance)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }
    }
}