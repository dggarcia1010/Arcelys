using UnityEngine;
using UnityEngine.Tilemaps;

public class IceTriggerSpell : MonoBehaviour
{
    [Header("Configuración del Sistema de Hielo")]
    [SerializeField] private Tilemap iceOverlayTilemap;  // ← arrastra el Tilemap de hielo aquí
    [SerializeField] private GameObject colliderToDisable; // barrera / puerta / lo que sea
    
    [Header("Efecto Mágico")]
    [SerializeField] private bool useMagicEffect = true;
    [SerializeField] private GameObject magicEffectPrefab; // Prefab con MagicSpriteEffect
    [SerializeField] private Vector3 effectOffset = Vector3.up * 0.5f;
    
    [Header("Sonidos")]
    [SerializeField] private AudioClip activationSound;
    
    private bool hasBeenActivated = false;
    private AudioSource audioSource;

    void Start()
    {
        // Configurar AudioSource si hay sonido
        if (activationSound != null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasBeenActivated) return;
        
        if (!other.CompareTag("IceSpell"))
            return;

        // Marcar como activado inmediatamente
        hasBeenActivated = true;

        // Opcional: destruir el hechizo
        Destroy(other.gameObject);

        // Iniciar la activación del puzzle
        StartCoroutine(ActivateIceMechanism());
    }

    private System.Collections.IEnumerator ActivateIceMechanism()
    {
        // 1. Mostrar efecto mágico si está configurado
        MagicSpriteEffect magicEffectInstance = null;
        
        if (useMagicEffect)
        {
            if (magicEffectPrefab != null)
            {
                // Instanciar el efecto en la posición del trigger
                Vector3 effectPosition = transform.position + effectOffset;
                GameObject effectObj = Instantiate(magicEffectPrefab, effectPosition, Quaternion.identity);
                magicEffectInstance = effectObj.GetComponent<MagicSpriteEffect>();
                
                // Opcional: si el prefab no tiene el script, intentar obtenerlo de este objeto
                if (magicEffectInstance == null)
                {
                    magicEffectInstance = GetComponent<MagicSpriteEffect>();
                }
            }
            else
            {
                // Buscar MagicSpriteEffect en este GameObject
                magicEffectInstance = GetComponent<MagicSpriteEffect>();
            }
            
            // Si tenemos un efecto, esperar a que termine antes de activar el hielo
            if (magicEffectInstance != null)
            {
                // Calcular tiempo total del efecto
                float totalEffectTime = magicEffectInstance.fadeInDuration + 
                                      magicEffectInstance.riseDuration + 
                                      magicEffectInstance.glowDuration;
                
                // Iniciar efecto
                magicEffectInstance.TriggerEffect();
                
                // Esperar hasta que el efecto termine (o parte de él)
                yield return new WaitForSeconds(magicEffectInstance.fadeInDuration + 
                                               magicEffectInstance.riseDuration);
            }
        }
        
        // 2. Reproducir sonido
        if (activationSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(activationSound);
        }
        
        // 3. Activar la capa visual de hielo
        if (iceOverlayTilemap != null)
        {
            iceOverlayTilemap.gameObject.SetActive(true);
            Debug.Log("Capa de hielo activada!");
            
            // Opcional: efecto de transición para el tilemap
            StartCoroutine(FadeInTilemap(iceOverlayTilemap, 0.5f));
        }
        
        // 4. Desactivar el collider/bloqueo
        if (colliderToDisable != null)
        {
            StartCoroutine(DeactivateObstacleWithDelay(0.2f));
        }
        
        // 5. Desactivar este trigger para evitar múltiples activaciones
        Collider2D myCollider = GetComponent<Collider2D>();
        if (myCollider != null)
        {
            myCollider.enabled = false;
        }
        
        // Opcional: cambiar apariencia visual del activador
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color c = sr.color;
            c.a = 0.3f;
            sr.color = c;
        }
    }
    
    private System.Collections.IEnumerator FadeInTilemap(Tilemap tilemap, float duration)
    {
        // Guardar color original
        Color originalColor = tilemap.color;
        Color transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        
        // Iniciar transparente
        tilemap.color = transparentColor;
        
        // Fade in gradual
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, originalColor.a, timer / duration);
            tilemap.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        
        // Restaurar color original
        tilemap.color = originalColor;
    }
    
    private System.Collections.IEnumerator DeactivateObstacleWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // Primero desactivar el collider
        Collider2D col = colliderToDisable.GetComponent<Collider2D>();
        if (col != null) 
            col.enabled = false;
        
        // Luego hacer fade out visual si tiene renderer
        SpriteRenderer sr = colliderToDisable.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color originalColor = sr.color;
            float timer = 0f;
            float fadeDuration = 0.5f;
            
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
                sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null;
            }
            
            // Finalmente desactivar el objeto completamente
            colliderToDisable.SetActive(false);
        }
        else
        {
            // Si no tiene renderer, desactivar inmediatamente
            colliderToDisable.SetActive(false);
        }
        
        Debug.Log("Barrera desactivada");
    }
    
    // Método para testing desde el editor
    [ContextMenu("Activar Manualmente")]
    public void ManualActivate()
    {
        if (!hasBeenActivated)
        {
            hasBeenActivated = true;
            StartCoroutine(ActivateIceMechanism());
        }
    }
    
    void OnDrawGizmos()
    {
        // Visualización en editor
        if (hasBeenActivated)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
        else
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.8f);
            
            // Mostrar conexiones
            if (iceOverlayTilemap != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, iceOverlayTilemap.transform.position);
            }
            
            if (colliderToDisable != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, colliderToDisable.transform.position);
            }
        }
    }
}
