using UnityEngine;
using UnityEngine.Rendering.Universal;   
using System.Collections;

public class MagicKeyOrbiter : MonoBehaviour
{
    public Transform center;              // La puerta (Door) como centro de órbita
    public float orbitRadius = 1.2f;      // Distancia al centro
    public float orbitSpeed = 120f;       // Grados por segundo
    public float glowIntensity = 2f;      // Intensidad final del brillo
    public float glowDuration = 1.5f;     // Tiempo que tarda en brillar fuerte

    private float currentAngle = 0f;
    private Light2D pointLight;           // Requiere Universal Render Pipeline + 2D Renderer
    private float initialIntensity = 1f;
    private bool isGlowing = false;

    void Start()
    {
        if (center == null)
        {
            center = GameObject.Find("Door")?.transform;
            if (center == null) Debug.LogError("MagicKeyOrbiter: No se encontró 'Door'");
        }

        pointLight = GetComponentInChildren<Light2D>();
        if (pointLight != null)
        {
            initialIntensity = pointLight.intensity;
            pointLight.intensity = 0f; // Empieza apagado o tenue
        }

        // Posición inicial
        transform.position = center.position + new Vector3(orbitRadius, 0, 0);
    }

    void Update()
    {
        if (center == null) return;

        // Rotación orbital
        currentAngle += orbitSpeed * Time.deltaTime;
        float x = Mathf.Cos(currentAngle * Mathf.Deg2Rad) * orbitRadius;
        float y = Mathf.Sin(currentAngle * Mathf.Deg2Rad) * orbitRadius;
        transform.position = center.position + new Vector3(x, y, 0);

        // Rotación propia de la llave (opcional, para que gire sobre sí misma)
        transform.Rotate(0, 0, 180f * Time.deltaTime); // Ajusta velocidad
    }

    // Llamar esto justo antes del fade
    public void StartFinalGlow()
    {
        if (isGlowing) return;
        isGlowing = true;
        StartCoroutine(GlowUp());
    }

    private System.Collections.IEnumerator GlowUp()
    {
        float timer = 0f;
        while (timer < glowDuration)
        {
            timer += Time.deltaTime;
            float t = timer / glowDuration;
            if (pointLight != null)
            {
                pointLight.intensity = Mathf.Lerp(initialIntensity, glowIntensity, t);
            }
            yield return null;
        }
    }
}
