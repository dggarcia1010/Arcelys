using UnityEngine;

public class VueloTrayectoriasLargas : MonoBehaviour
{
    [Header("Objetivo")]
    [Tooltip("El objeto central alrededor del cual el bicho vuela.")]
    public Transform target; 

    [Header("Configuración del Vuelo")]
    [Tooltip("La distancia máxima de alejamiento. Define el radio de la trayectoria.")]
    public float flightRange = 8f; // Aumentado para mayor distancia de alejamiento

    [Tooltip("La velocidad con la que el insecto recorre la trayectoria (ritmo de cambio).")]
    public float speed = 0.5f; // Reducido para trayectorias más suaves y largas

    [Tooltip("Suavidad de la transición. Un valor más alto hace que el movimiento sea más responsivo.")]
    public float smoothness = 4f; // Ligeramente ajustado para una respuesta fluida

    // Compensaciones iniciales para el Ruido de Perlin.
    private float noiseOffsetX;
    private float noiseOffsetY;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Error: NO HAY TARGET");
            enabled = false;
            return;
        }

        // Elegir un punto de inicio aleatorio en la función de Ruido de Perlin.
        noiseOffsetX = Random.Range(0f, 100f);
        noiseOffsetY = Random.Range(0f, 100f);
    }

    void Update()
    {
        if (target == null) return;

        // --- 1. Muestrear el Ruido de Perlin ---
        
        // El valor de 'speed' bajo (0.5f) asegura que el cambio en el tiempo sea lento,
        // resultando en un camino más largo y menos zigzagueante.
        float timeFactorX = Time.time * speed + noiseOffsetX;
        float timeFactorY = Time.time * speed + noiseOffsetY;

        float noiseValueX = Mathf.PerlinNoise(timeFactorX, 0f);
        float noiseValueY = Mathf.PerlinNoise(timeFactorY, 0f);

        // --- 2. Mapear y Escalar a la Posición ---
        
        // El 'flightRange' alto (8f) asegura que el bicho cubra una gran área,
        // alejándose significativamente del centro.
        float posX = (noiseValueX * 2f - 1f) * flightRange;
        float posY = (noiseValueY * 2f - 1f) * flightRange;

        // La posición de destino está centrada en la posición del objetivo (target).
        Vector3 destination = target.position + new Vector3(posX, posY, 0);

        // --- 3. Aplicar la Posición Final ---
        
        // Mover suavemente el objeto hacia la nueva posición en el camino.
        transform.position = Vector3.Lerp(
            transform.position, 
            destination, 
            Time.deltaTime * smoothness
        );
    }
}
