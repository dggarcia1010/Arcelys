using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Projectile2D : MonoBehaviour
{
    // =========================
    // EQUIPOS / OWNER
    // =========================
    public enum Team { Player, Enemy }

    [Header("Equipo / Owner")]
    public Team team = Team.Player;
    public Collider2D ownerCollider;
    public float spawnOffset = 0.3f;

    // =========================
    // MOVIMIENTO
    // =========================
    [Header("Movimiento")]
    public Vector2 direction = Vector2.up;
    public float speed = 5f;
    public float lifeTime = 2f;

    // =========================
    // ROTACIÓN VISUAL
    // =========================
    [Header("Rotación visual")]
    public bool rotateToDirection = true;

    // Sprite mirando a la DERECHA: 0
    // Sprite mirando ARRIBA: -90
    public float spriteAngleOffset = 0f;

    // =========================
    // DAÑO
    // =========================
    [Header("Daño base")]
    public int damage = 1;

    // =========================
    // WIND
    // =========================
    [Header("Wind")]
    public bool isWind = false;
    public string pushableTag = "Pushable";
    public float windPushForce = 1f;

    // =========================
    // FIRE
    // =========================
    [Header("Fire")]
    public bool isFire = false;
    public string flammableTag = "Flammable";

    // =========================
    // ICE
    // =========================
    [Header("Ice")]
    public bool isIce = false;
    public float slowMultiplier = 0.4f;
    public float slowDuration = 3f;

    // =========================
    // START
    // =========================
    void Start()
    {
        // Mover un poco hacia delante para no nacer dentro del disparador
        transform.position += (Vector3)(direction.normalized * spawnOffset);

        // Ignorar colisión con quien disparó
        if (ownerCollider != null)
        {
            Collider2D myCol = GetComponent<Collider2D>();
            if (myCol != null)
                Physics2D.IgnoreCollision(myCol, ownerCollider, true);
        }

        // Rotación visual
        if (rotateToDirection && !isWind)
            ApplyRotation();

        // Autodestrucción
        Destroy(gameObject, lifeTime);
    }

    // =========================
    // UPDATE
    // =========================
    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    // =========================
    // ROTACIÓN
    // =========================
    void ApplyRotation()
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + spriteAngleOffset);
    }

    // =========================
    // COLISIONES
    // =========================
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Nunca dañar al owner
        if (ownerCollider != null && collision == ownerCollider)
            return;

        // =====================
        // PROYECTIL DEL ENEMIGO
        // =====================
        if (team == Team.Enemy && collision.CompareTag("Player"))
        {
            HealthSystem hs = collision.GetComponent<HealthSystem>();
            if (hs != null)
                hs.TakeDamage(damage);

            Destroy(gameObject);
            return;
        }

        // =====================
        // PROYECTIL DEL PLAYER
        // =====================
        if (team == Team.Player && collision.CompareTag("Enemy"))
        {
            EnemyHealth hp = collision.GetComponent<EnemyHealth>();
            if (hp != null)
                hp.TakeDamage(damage);

            // ❄ HIELO
            if (isIce)
            {
                EnemySlow slowComponent = collision.GetComponent<EnemySlow>();
                if (slowComponent != null)
                    slowComponent.ApplySlow(slowMultiplier, slowDuration);
            }

            // 🌪 VIENTO
            if (isWind)
            {
                Rigidbody2D rbEnemy = collision.attachedRigidbody;
                if (rbEnemy != null)
                {
                    Vector2 pushDir =
                        ((Vector2)collision.transform.position - (Vector2)transform.position).normalized;

                    rbEnemy.AddForce(pushDir * windPushForce, ForceMode2D.Impulse);
                }
            }

            Destroy(gameObject);
            return;
        }

        // =====================
        // SOLO PROYECTILES DEL PLAYER
        // =====================
        if (team == Team.Player)
        {
            // OBJETOS EMPUJABLES
            if (isWind && collision.CompareTag(pushableTag))
            {
                Vector2 pushDir =
                    ((Vector2)collision.transform.position - (Vector2)transform.position).normalized;

                Pushable pushable = collision.GetComponent<Pushable>();
                if (pushable != null)
                    pushable.UnlockAndPush(pushDir, windPushForce);
                else
                {
                    Rigidbody2D rb = collision.attachedRigidbody;
                    if (rb != null)
                        rb.AddForce(pushDir * windPushForce, ForceMode2D.Impulse);
                }

                Destroy(gameObject);
                return;
            }

            // OBJETOS INFLAMABLES
            if (isFire && collision.CompareTag(flammableTag))
            {
                Light2D light2D =
                    collision.GetComponent<Light2D>() ??
                    collision.GetComponentInChildren<Light2D>(true) ??
                    collision.GetComponentInParent<Light2D>(true);

                if (light2D != null)
                    light2D.enabled = true;

                FlammableTorch torch = collision.GetComponent<FlammableTorch>();
                if (torch != null)
                    torch.TurnOn();

                Destroy(gameObject);
                return;
            }
        }
    }
}