using UnityEngine;
using System.Collections;

public class MagicSpriteEffect : MonoBehaviour
{
    [Header("Duraciones (segundos)")]
    public float fadeInDuration = 1.0f;      // Aparición gradual
    public float riseDuration = 1.5f;        // Tiempo subiendo
    public float glowDuration = 2.0f;        // Tiempo brillando (pulsando escala)
    public float fadeOutDuration = 1.0f;     // Desaparición

    [Header("Efectos visuales")]
    public float riseHeight = 2.0f;          // Cuánto sube en Y
    public float glowScaleMin = 1.0f;        // Escala mínima durante el pulso (normal)
    public float glowScaleMax = 1.3f;        // Escala máxima durante el pulso

    [Header("Final")]
    public bool destroyOnEnd = true;         // Destruir al terminar

    private SpriteRenderer spriteRenderer;
    private Vector3 initialPosition;
    private Vector3 initialScale;
    private Color initialColor;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("MagicSpriteEffect necesita un SpriteRenderer!");
            return;
        }

        initialPosition = transform.position;
        initialScale = transform.localScale;
        initialColor = spriteRenderer.color;

        // Iniciar el efecto automáticamente
        StartCoroutine(PlayMagicEffect());
    }

    private IEnumerator PlayMagicEffect()
    {
        // 1. FADE IN (aparece gradualmente)
        spriteRenderer.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);
        float timer = 0f;
        while (timer < fadeInDuration)
        {
            timer += Time.deltaTime;
            float alpha = timer / fadeInDuration;
            spriteRenderer.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            yield return null;
        }
        spriteRenderer.color = initialColor;

        // 2. SE ALZA HACIA ARRIBA
        Vector3 targetRisePos = initialPosition + Vector3.up * riseHeight;
        timer = 0f;
        while (timer < riseDuration)
        {
            timer += Time.deltaTime;
            float t = timer / riseDuration;
            t = Mathf.Sin(t * Mathf.PI * 0.5f); // Curva suave
            transform.position = Vector3.Lerp(initialPosition, targetRisePos, t);
            yield return null;
        }
        transform.position = targetRisePos;

        // 3. BRILLO: solo pulsa en escala, sin cambiar color
        timer = 0f;
        while (timer < glowDuration)
        {
            timer += Time.deltaTime;
            float t = timer / glowDuration;

            // Pulso sinusoidal suave
            float pulse = Mathf.Sin(t * Mathf.PI * 4f) * 0.5f + 0.5f; // 2 ciclos completos

            float scale = Mathf.Lerp(glowScaleMin, glowScaleMax, pulse);
            transform.localScale = initialScale * scale;

            yield return null;
        }

        // Restaurar escala original
        transform.localScale = initialScale;

        // 4. FADE OUT (desaparece)
        timer = 0f;
        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;
            float alpha = 1f - (timer / fadeOutDuration);
            spriteRenderer.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            yield return null;
        }

        // Final
        spriteRenderer.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);
        transform.position = initialPosition;

        if (destroyOnEnd)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);

        Debug.Log("Efecto mágico completado (solo escala pulsante)");
    }

    // Método público para activarlo manualmente (ej: con E)
    public void TriggerEffect()
    {
        StartCoroutine(PlayMagicEffect());
    }
}
