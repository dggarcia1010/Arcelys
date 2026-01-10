using UnityEngine;
using System.Collections; // Necesario para usar IEnumerator

public class EnemyAttack : MonoBehaviour
{
    public float attackDamage = 1f;

    public float attackCooldown = 1.0f; 

    public float attackDuration = 0.5f; // 💡 NUEVA VARIABLE: Duración del ataque real
    
    // Variables privadas de estado
    private float lastAttackTime;
    private bool isAttacking = false; // Controla si la secuencia de ataque está activa
    
    void Start()
    {
        Debug.Log("EnemyAttack iniciado - Daño: " + attackDamage + ", Cooldown: " + attackCooldown + ", Duración: " + attackDuration);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryAttack(other);
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        TryAttack(other);
    }

    private void TryAttack(Collider2D other)
    {
        // 1. RESTRICCIÓN: No puede empezar un nuevo ataque si ya está en la secuencia de uno.
        if (isAttacking)
        {
            // Debug.Log("Ataque pendiente: Ya estamos en la secuencia de ataque.");
            return;
        }

        // 2. Verificar si el objetivo es el jugador y si el cooldown ha terminado.
        if (other.CompareTag("Player") && Time.time > lastAttackTime + attackCooldown)
        {
            Debug.Log("Ataque exitoso a jugador - Cooldown completado. INICIANDO ATAQUE.");
            
            // 3. Establece el estado: Ahora estamos atacando.
            isAttacking = true;
            
            // 4. Inicia la corutina que manejará la duración del ataque.
            StartCoroutine(AttackSequenceCoroutine(other));
        }
    }

   // Corutina que simula la duración de la animación/secuencia de ataque.
    
    private IEnumerator AttackSequenceCoroutine(Collider2D target)
    {
        // 1. Lógica de daño (ocurre inmediatamente al inicio de la secuencia)
        HealthSystem playerHealth = target.GetComponent<HealthSystem>();

        if (playerHealth != null)
        {
            // Infligir daño
            playerHealth.TakeDamage(attackDamage);
            
            // Reiniciar el tiempo de cooldown.
            lastAttackTime = Time.time; 
            Debug.Log("Daño infligido. Esperando duración de ataque (" + attackDuration + "s) y luego cooldown.");
        }
        else
        {
            Debug.LogWarning("HealthSystem no encontrado en el jugador.");
        }

        // 2. Esperar la duración del ataque (simula el tiempo de la animación).
        yield return new WaitForSeconds(attackDuration);

        // 3. Finalizar el ataque: permitir que se inicie el siguiente.
        FinishAttack();
    }

    
    // Método para finalizar la secuencia de ataque y resetear el estado
    private void FinishAttack()
    {
        isAttacking = false;
        Debug.Log("Secuencia de ataque terminada. El enemigo puede intentar un nuevo ataque ahora (si el cooldown lo permite).");
    }
}
