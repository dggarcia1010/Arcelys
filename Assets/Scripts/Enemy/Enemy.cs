using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Configuración Base")]
    public float speed = 3f;           // Velocidad máxima de movimiento del enemigo.
    public float maxForce = 10f;       // Máxima fuerza de dirección para el movimiento basado en física.

    protected Transform playerTransform; // Referencia al Transform del jugador (Target).
    protected Rigidbody2D rb;          // Componente de física para movimiento.
    protected Animator animator;         // Componente Animator para controlar las animaciones.
    
    // Bandera para bloquear movimiento durante el ataque
    protected bool isAttacking = false; 
    
    // Variables para el Animator (Idle/Walk/Dirección)
    protected float currentSpeed = 0f;  
    protected Vector2 currentDirection = new Vector2(0, -1); // Inicializado hacia abajo por defecto.

    protected virtual void Start()
    {
        // 1. Inicializar Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError($"[ID: {gameObject.name}] Rigidbody2D es necesario para el movimiento. Desactivando script.");
            enabled = false;
            return;
        }
        
        // 2. Inicializar Animator
        animator = GetComponent<Animator>(); 
        if (animator == null)
        {
            Debug.LogError($"[ID: {gameObject.name}] Animator no encontrado.");
        }
        
        // Configuraciones de Rigidbody que tenemos que poner para 2D Top-Down
        rb.gravityScale = 0f;
        rb.freezeRotation = true; 

        // 3. Buscar al jugador (Target)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            Debug.Log($"Jugador '{player.name}' encontrado para {gameObject.name}.");
        }
        else
        {
            Debug.LogError("No se encontró el objeto 'Player'. Asegúrate de que el jugador tiene el Tag 'Player'.");
        }
    }

    protected virtual void Move()    
    {
        // Implementación específica de la IA en clases derivadas (MeleeChaser).
    }
    
    protected virtual void UpdateAnimator()
    {
        if (animator == null) return;
        
        // 1. Controla la transición Idle <-> Walk
        animator.SetFloat("Speed", currentSpeed);
        
        // 2. Controla la dirección para los Blend Trees 2D (IdleTree y WalkTree)
        // Usa el último valor de currentDirection, incluso si la velocidad es 0.
        animator.SetFloat("moveX", currentDirection.x);
        animator.SetFloat("moveY", currentDirection.y);
        
        // 3. Controla la transición de Ataque
        animator.SetBool("IsAttacking", isAttacking); 
    }

    void FixedUpdate()
    {
        // Lógica de Movimiento: Solo se ejecuta si el enemigo no está en un estado de ataque.
        if (!isAttacking) 
        {
            Move();
            
            // Calculamos la velocidad real del Rigidbody
            currentSpeed = rb.linearVelocity.magnitude;
            
            // Lógica de Persistencia: SOLO actualiza la dirección si el enemigo se está moviendo.
            // Si currentSpeed es <= 0.1f, currentDirection MANTIENE el último valor.
            if (currentSpeed > 0.1f)
            {
                currentDirection = rb.linearVelocity.normalized;
            }
        }
        else
        {
            // Si está atacando, la velocidad para el Animator es cero
            currentSpeed = 0f;
            // IMPORTANTE: currentDirection MANTIENE la última dirección para el AttackTree.
        }
    }
    
    void Update() 
    {
        // Actualizamos las animaciones.
        UpdateAnimator();
    }
}
