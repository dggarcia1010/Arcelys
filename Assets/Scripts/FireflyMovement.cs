using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FireflyWander_Simple : MonoBehaviour
{
    [Header("Configuración de Restricción")]
    [Tooltip("Radio máximo de alejamiento del punto de inicio.")]
    public float maxRadius = 6f; // El valor que solicitaste

    [Header("Configuración del Vuelo Libre")]
    public float movementSpeed = 0.2f; 
    public float noiseScale = 0.5f;
    public float movementSmoothness = 2f; 
    
    private Vector3 startPosition; // ¡Almacena el punto de inicio!
    private float noiseOffsetX;
    private float noiseOffsetY;

    [Header("Configuración de la Luz")]
    public Light2D fireflyLight; 
    public float baseLightIntensity = 1.2f;
    public float flickerMagnitude = 0.4f;
    public float flickerSpeed = 12f;

    void Start()
    {
        // 1. Guarda la posición inicial como el centro del movimiento.
        startPosition = transform.position; 
        
        noiseOffsetX = Random.Range(0f, 100f);
        noiseOffsetY = Random.Range(0f, 100f);

        if (fireflyLight == null)
        {
            fireflyLight = GetComponent<Light2D>();
        }
    }

    void Update()
    {
        // --- 1. Calcular la POSICIÓN DESEADA (Wander) ---
        float time = Time.time * movementSpeed;
        float rawX = Mathf.PerlinNoise(time, noiseOffsetX);
        float rawY = Mathf.PerlinNoise(time, noiseOffsetY);

        float dirX = rawX * 2f - 1f;
        float dirY = rawY * 2f - 1f;

        // Calcula el destino NO RESTRINGIDO por el ruido de Perlin
        Vector3 randomDirection = new Vector3(dirX, dirY, 0).normalized * noiseScale;
        Vector3 targetPosition = transform.position + randomDirection;

        // --- 2. APLICAR EL LÍMITE DE RADIO (6 unidades) ---
        
        // 2a. Calcula el vector de desplazamiento desde el punto de inicio.
        Vector3 displacementFromStart = targetPosition - startPosition;
        
        // 2b. ClampMagnitude: Si la magnitud (distancia) es mayor que 'maxRadius', 
        // reduce el vector al radio máximo. Si es menor, lo deja igual.
        displacementFromStart = Vector3.ClampMagnitude(displacementFromStart, maxRadius);
        
        // 2c. Calcula la posición final restringida.
        Vector3 restrictedPosition = startPosition + displacementFromStart;

        // --- 3. Aplicar el Movimiento ---
        transform.position = Vector3.Lerp(
            transform.position, 
            restrictedPosition, // Mueve hacia la posición restringida
            Time.deltaTime * movementSmoothness
        );

        // --- 4. EFECTO DE LUZ PARPADEANTE ---
        if (fireflyLight != null)
        {
            float flicker = Mathf.Sin(Time.time * flickerSpeed);
            float currentIntensity = baseLightIntensity + (flicker * flickerMagnitude);
            fireflyLight.intensity = Mathf.Max(0, currentIntensity);
        }
    }
}
