using UnityEngine;
using System.Collections; 

public class MeleeChaser : Enemy
{
    public float stoppingDistance = 1.0f; 
    public float slowdownRadius = 3.0f;
    public float maxChaseDistance = 10.0f; 

    public float attackRange = 1.0f; 
    public float attackDuration = 0.5f; 
    public float attackCooldown = 1.5f; 
    private float timeUntilNextAttack = 0f; 

    protected override void Start()
    {
        base.Start();
        attackRange = Mathf.Min(attackRange, stoppingDistance); 
    }

    protected override void Move()
    {
        if (playerTransform == null || rb == null) return;

        Vector2 targetPosition = playerTransform.position;
        Vector2 currentPosition = rb.position;
        float distanceToPlayer = Vector2.Distance(currentPosition, targetPosition);
        
        // 0. LÓGICA DE DETENCIÓN LEJANA
        // Si el jugador está demasiado lejos, nos detenemos y no hacemos nada más.
        if (distanceToPlayer > maxChaseDistance)
        {
            rb.linearVelocity = Vector2.zero; 
            return; 
        }
        
        // 1. LÓGICA DE ATAQUE 
        if (distanceToPlayer <= attackRange && Time.time >= timeUntilNextAttack)
        {
            // Detener el movimiento y empezar el ataque
            rb.linearVelocity = Vector2.zero;
            StartCoroutine(AttackCo());
            return; 
        }
        
        // 2. LÓGICA DE MOVIMIENTO (Persecución/Llegada)
        
        Vector2 desiredVelocity;

        if (distanceToPlayer > attackRange) 
        {
            Vector2 direction = (targetPosition - currentPosition).normalized;
            
            if (distanceToPlayer < slowdownRadius)
            {
                float desiredSpeed = speed * (distanceToPlayer / slowdownRadius);
                desiredVelocity = direction * desiredSpeed;
            }
            else
            {
                desiredVelocity = direction * speed;
            }

            Vector2 steeringForce = desiredVelocity - rb.linearVelocity;
            steeringForce = Vector2.ClampMagnitude(steeringForce, maxForce);
            rb.AddForce(steeringForce);
        }
        else 
        {
             // Estamos en rango, pero en cooldown: nos paramos
             rb.linearVelocity = Vector2.zero;
        }
    }
    
    private IEnumerator AttackCo()
    {
        isAttacking = true;
        
        // El Rigidbody ya está parado desde Move()
        
        // La lógica de la hitbox del enemigo iría aquí.
        yield return new WaitForSeconds(attackDuration);
        
        isAttacking = false;
        
        timeUntilNextAttack = Time.time + attackCooldown;
    }
}
