using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Projectile2D : MonoBehaviour
{
    [Header("Movimiento")]
    public Vector2 direction = Vector2.up;
    public float speed = 5f;
    public float lifeTime = 2f;

    [Header("Daño base")]
    public int damage = 1; // cuánto quita el proyectil

    [Header("Wind")]
    public bool isWind = false;
    public string pushableTag = "Pushable";
    public float windPushForce = 1f; // fuerza del empuje

    [Header("Fire")]
    public bool isFire = false;
    public string flammableTag = "Flammable";

    [Header("Ice")]
    public bool isIce = false;
    public float slowMultiplier = 0.4f; // 40% velocidad
    public float slowDuration = 3f;     // 3 segundos

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1) Si choco con un ENEMIGO
        if (collision.CompareTag("Enemy"))
        {
            EnemyHealth hp = collision.GetComponent<EnemyHealth>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
            }

            // ❄ HIELO -> ralentiza
            if (isIce)
            {
                EnemySlow slowComponent = collision.GetComponent<EnemySlow>();
                if (slowComponent != null)
                {
                    slowComponent.ApplySlow(slowMultiplier, slowDuration);
                }
            }

            // 🌪 VIENTO -> empuja al enemigo también
            if (isWind)
            {
                var rbEnemy = collision.attachedRigidbody;
                if (rbEnemy != null)
                {
                    Vector2 pushDir = ((Vector2)collision.transform.position - (Vector2)transform.position).normalized;
                    rbEnemy.AddForce(pushDir * windPushForce, ForceMode2D.Impulse);
                }
            }

            Destroy(gameObject);
            return;
        }

        // 2) Si choco con objeto "Pushable" (cajas, etc)

        if (isWind && collision.CompareTag(pushableTag))
        {
            // Dirección desde el proyectil hacia el objeto
            Vector2 pushDir = ((Vector2)collision.transform.position - (Vector2)transform.position).normalized;

            // Si tiene componente Pushable -> desbloquear, empujar y re-bloquear
            var pushable = collision.GetComponent<Pushable>();
            if (pushable != null)
            {
                pushable.UnlockAndPush(pushDir, windPushForce);
            }
            else
            {
                // Fallback (por si olvidaste añadir el componente)
                var rb = collision.attachedRigidbody;
                if (rb != null) rb.AddForce(pushDir * windPushForce, ForceMode2D.Impulse);
            }

            Destroy(gameObject);
            return;
        }

        // 3) Si choco con objeto "Flammable" (antorcha)

        if (isFire && collision.CompareTag(flammableTag))
        {
            // encender luz visual
            Light2D light2D =
                collision.GetComponent<Light2D>() ??
                collision.GetComponentInChildren<Light2D>(true) ??
                collision.GetComponentInParent<Light2D>(true);

            if (light2D != null)
            {
                light2D.enabled = true;
            }

            // informar al puzzle
            var torch = collision.GetComponent<FlammableTorch>();
            if (torch != null)
            {
                torch.TurnOn();
            }

            Destroy(gameObject);
            return;
        }
    }
}