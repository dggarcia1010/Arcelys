using UnityEngine;
using UnityEngine.Events;
using System.Collections; 

public class HealthSystem : MonoBehaviour
{
    public float maxHealth = 10f;  
    public float currentHealth;    
    
    // Indica si el personaje no recibe daño temporalmente
    private bool isInvincible = false;
    public float invincibilityDuration = 1f;

    // Feedback Visual 
    public SpriteRenderer playerSpriteRenderer; 
    public Color damageColor = Color.red;       
    public float flashDuration = 0.1f;          
    public float blinkInterval = 0.1f;
    private Color originalColor;                

    // Eventos (Para UI y Game Over)
    public UnityEvent onDamageTaken; 
    public UnityEvent onDeath;       

    private bool isDead = false;
    private Coroutine invincibilityCoroutine;

    private Collider2D playerCollider;
    private PlayerMovement playerMovement;
    private Rigidbody2D rb;

    void Start()
    {
        currentHealth = maxHealth;
        
        if (playerSpriteRenderer == null)
        {
            playerSpriteRenderer = GetComponent<SpriteRenderer>();
        }

        playerCollider = GetComponent<Collider2D>();
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();

        if (playerSpriteRenderer != null)
        {
            originalColor = playerSpriteRenderer.color;
        }
        
        Debug.Log("HealthSystem iniciado - Vida: " + currentHealth + "/" + maxHealth);
    }

    public void TakeDamage(float damage)
    {
        if (isDead || isInvincible) 
        {
            Debug.Log("Daño bloqueado - Invincible: " + isInvincible + ", Muerto: " + isDead);
            return; 
        }

        Debug.Log("¡Daño recibido! " + damage + " puntos. Vida anterior: " + currentHealth);
        
        currentHealth -= damage;
        onDamageTaken.Invoke(); 

        Debug.Log("Vida actual: " + currentHealth + "/" + maxHealth);

        // 1. Flash de daño inmediato (rojo)
        StartCoroutine(FlashDamageFeedback()); 
        
        // 2. Invencibilidad y parpadeo
        if (invincibilityCoroutine != null)
            StopCoroutine(invincibilityCoroutine);
            
        invincibilityCoroutine = StartCoroutine(BecomeTemporarilyInvincible(invincibilityDuration));

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float healing)
    {
        if (isDead) 
        {
            Debug.Log("Intento de curación fallido - Jugador muerto");
            return;
        }
        
        Debug.Log("Curación recibida: " + healing + " puntos. Vida anterior: " + currentHealth);
        
        currentHealth += healing;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        onDamageTaken.Invoke(); 
        
        Debug.Log("Vida después de curación: " + currentHealth + "/" + maxHealth);
    }
    
    public void Die()
    {
        if (isDead) return; // Evitar múltiples llamadas a Die()
        
        isDead = true;
        currentHealth = 0; 

        onDeath.Invoke(); 
        
        Debug.Log(transform.name + " ha muerto. Desactivando componentes...");

        // Deshabilitar movimiento
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
            Debug.Log("PlayerMovement deshabilitado");
        }
        
        // Deshabilitar collider para que no interactúe con enemigos
        if (playerCollider != null)
        {
            playerCollider.enabled = false;
            Debug.Log("Collider2D deshabilitado");
        }
        
        // Detener cualquier movimiento físico
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false; // Importante para Rigidbody2D
            Debug.Log("Rigidbody2D detenido y deshabilitado");
        }
        
        // Deshabilitar el sprite renderer
        if (playerSpriteRenderer != null)
        {
            playerSpriteRenderer.enabled = false;
            Debug.Log("SpriteRenderer deshabilitado");
        }
    }

    private IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
        Debug.Log("GameObject desactivado completamente");
    }
    
    private IEnumerator BecomeTemporarilyInvincible(float duration)
    {
        isInvincible = true;
        Debug.Log("Invencibilidad ACTIVADA por " + duration + " segundos");
        
        // Espera el flash rojo antes de empezar a parpadear
        yield return new WaitForSeconds(flashDuration);
        
        // Inicia el parpadeo después del flash
        StartCoroutine(InvincibilityFlash(duration - flashDuration));
        
        yield return new WaitForSeconds(duration - flashDuration);
        
        isInvincible = false;
        Debug.Log("Invencibilidad DESACTIVADA");
        
        // Aseguramos que el sprite vuelve a estar visible y con su color original
        if (playerSpriteRenderer != null && !isDead) // Solo si no está muerto
        {
             playerSpriteRenderer.enabled = true; 
             playerSpriteRenderer.color = originalColor;
        }
    }

    private IEnumerator FlashDamageFeedback()
    {
        if (playerSpriteRenderer != null && !isDead) // Solo si no está muerto
        {
            Debug.Log("Iniciando flash rojo de daño");
            
            // Guarda el color original y aplica el color de daño
            playerSpriteRenderer.color = damageColor;
            yield return new WaitForSeconds(flashDuration);
            
            // Solo vuelve al color original si no estamos en modo invencible (donde parpadearemos) y no está muerto
            if (!isInvincible && playerSpriteRenderer != null && !isDead)
            {
                playerSpriteRenderer.color = originalColor;
            }
            
            Debug.Log("Flash rojo completado");
        }
    }
    
    
    private IEnumerator InvincibilityFlash(float duration)
    {
        float timer = 0f;
        Debug.Log("Iniciando parpadeo de invencibilidad por " + duration + " segundos");
        
        while (timer < duration && !isDead) // Parar si muere durante la invencibilidad
        {
            if (playerSpriteRenderer != null)
            {
                // Alterna entre visible (color original) y oculto/casi invisible
                playerSpriteRenderer.enabled = !playerSpriteRenderer.enabled;
                Debug.Log("Parpadeo - Sprite visible: " + playerSpriteRenderer.enabled);
            }
            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }
        
        // Aseguramos que el sprite es visible al final del parpadeo (solo si no está muerto)
        if (playerSpriteRenderer != null && !isDead)
        {
            playerSpriteRenderer.enabled = true;
            playerSpriteRenderer.color = originalColor;
        }
        
        Debug.Log("Parpadeo de invencibilidad completado");
    }

    // Método público para verificar si está muerto
    public bool IsDead()
    {
        return isDead;
    }
}
