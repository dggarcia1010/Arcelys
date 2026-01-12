using UnityEngine;

public class InsectosUltraMolestos : MonoBehaviour
{
    [Header("Objetivo principal (centro de órbita cuando estás lejos)")]
    public Transform target;

    [Header("--- Configuración normal ---")]
    public float flightRange = 9f;
    public float normalSpeed = 0.45f;
    public float normalSmoothness = 3.8f;

    [Header("--- ZONA DE AGRESIÓN MÁXIMA ---")]
    [Tooltip("Radio donde se activan y se vuelven insoportables")]
    public float aggroDistance = 4.2f;

    [Tooltip("Distancia objetivo ideal cuando están en modo 'te odio'")]
    public float targetCloseDistance = 0.4f;     // ¡muy cerca de la cara!

    [Tooltip("Velocidad multiplicador en modo molesto (muy rápido)")]
    public float aggroSpeedMult = 5.5f;

    [Tooltip("Suavidad multiplicador (muy bajo = muy nervioso y brusco)")]
    public float aggroSmoothMult = 0.25f;

    [Tooltip("Chance de intentar 'posarse' directamente en la cara cada frame")]
    [Range(0f, 1f)] public float chanceToDiveBomb = 0.55f;

    [Tooltip("Cuánto orbitan alrededor de tu cabeza cuando no dive-bomb")]
    public float orbitRadius = 1.6f;

    [Tooltip("Frecuencia de cambios erráticos extra cuando molestos")]
    public float jitterFrequency = 12f;

    [Tooltip("Intensidad del jitter (temblores rápidos)")]
    public float jitterAmount = 0.7f;

    private Transform player;
    private float noiseOffsetX, noiseOffsetY, noiseOffsetZ;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("¡No hay TARGET! Insecto desactivado.");
            enabled = false;
            return;
        }

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
            Debug.LogWarning("No se encontró Player → solo modo normal");

        noiseOffsetX = Random.Range(0f, 999f);
        noiseOffsetY = Random.Range(0f, 999f);
        noiseOffsetZ = Random.Range(0f, 999f);
    }

    void Update()
    {
        if (target == null) return;

        if (player == null || Vector3.Distance(transform.position, player.position) > aggroDistance)
        {
            NormalChillFlight();
        }
        else
        {
            MaximumAnnoyanceMode();
        }
    }

    private void NormalChillFlight()
    {
        float t = Time.time * normalSpeed;
        float x = (Mathf.PerlinNoise(t + noiseOffsetX, 0) * 2 - 1) * flightRange;
        float y = (Mathf.PerlinNoise(t + noiseOffsetY, 5) * 2 - 1) * flightRange;

        Vector3 dest = target.position + new Vector3(x, y, 0);
        transform.position = Vector3.Lerp(transform.position, dest, Time.deltaTime * normalSmoothness);
    }

    private void MaximumAnnoyanceMode()
    {
        Vector3 toPlayer = (player.position - transform.position);
        float dist = toPlayer.magnitude;
        Vector3 dir = toPlayer.normalized;

        Vector3 desiredPos;

        // 50/50 chance de dos comportamientos muy irritantes
        if (Random.value < chanceToDiveBomb)
        {
            // Modo KAMIKAZE: intenta posarse directamente en la cara
            desiredPos = player.position + Random.insideUnitSphere * 0.3f;
        }
        else
        {
            // Modo ORBITA LOCA + jitter
            Vector3 orbitOffset = Quaternion.Euler(0, 0, Time.time * 220f) * (Vector3.right * orbitRadius);
            desiredPos = player.position + orbitOffset + Random.insideUnitSphere * 0.6f;
        }

        // Jitter extra (temblores rápidos que vuelven loco al jugador)
        float jitter = Mathf.PerlinNoise(Time.time * jitterFrequency + noiseOffsetZ, 0) * jitterAmount * 2f - jitterAmount;
        desiredPos += dir * jitter * 0.4f;

        // Movimiento extremadamente rápido y poco suave
        float currentSmooth = normalSmoothness * aggroSmoothMult;
        float currentSpeed = normalSpeed * aggroSpeedMult;

        // Le damos un toque de ruido rápido incluso en la posición deseada
        float nt = Time.time * currentSpeed * 3.2f;
        Vector3 extraNoise = new Vector3(
            Mathf.PerlinNoise(nt + noiseOffsetX, 0) - 0.5f,
            Mathf.PerlinNoise(nt + noiseOffsetY, 10) - 0.5f,
            0
        ) * 1.8f;

        Vector3 finalTarget = desiredPos + extraNoise;

        transform.position = Vector3.Lerp(transform.position, finalTarget, Time.deltaTime * currentSmooth);
    }

    // Ayuda visual en Scene view
    void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = new Color(1f, 0.1f, 0.1f, 0.7f);
            Gizmos.DrawWireSphere(player.position, aggroDistance);

            Gizmos.color = new Color(1f, 0.4f, 0.1f, 0.4f);
            Gizmos.DrawWireSphere(player.position, targetCloseDistance);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(player.position, orbitRadius);
        }
    }
}
