using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Crystal3InteractUnlock : MonoBehaviour
{
    [Header("Input")]
    public KeyCode interactKey = KeyCode.E;
    public string playerTag = "Player";

    [Header("Diálogo (opcional)")]
    public Dialogue dialogueBeforeUnlock;

    [Header("Reliquia efecto (GameObject oculto con MagicSpriteEffect)")]
    public GameObject relicEffectObject;

    [Header("Aparece encima del jugador")]
    public Vector3 relicOffset = new Vector3(0f, 0.8f, 0f);

    [Header("Colliders tras el diálogo")]
    [Tooltip("Collider que se desactivará al terminar el diálogo")]
    public Collider2D colliderToDisable;

    [Tooltip("Collider que se activará al terminar el diálogo (ej: ColliderAtrás)")]
    public Collider2D colliderToEnable;

    [Tooltip("Al activar el colliderToEnable, forzarlo como sólido (isTrigger = false)")]
    public bool enableColliderAsSolid = true;

    [Header("Ocultar reliquia visible al pulsar E")]
    [Tooltip("Si está activo, la reliquia visible desaparece al pulsar E, sin esperar a la animación.")]
    public bool hideWorldRelicImmediatelyOnInteract = true;

    [Tooltip("Si lo asignas, se ocultará este objeto. Si no, se ocultará SpriteRenderer + Collider del propio objeto.")]
    public GameObject worldRelicVisualRoot;

    [Header("Comportamiento")]
    public bool freezePlayerDuringEffect = true;
    public bool oneTime = true;

    private bool playerInRange = false;
    private bool used = false;

    private Transform playerTransform;
    private MonoBehaviour playerMovement;

    // Componentes del propio objeto (por si no asignas worldRelicVisualRoot)
    private SpriteRenderer mySprite;
    private Collider2D myTriggerCollider;

    void Awake()
    {
        myTriggerCollider = GetComponent<Collider2D>();
        myTriggerCollider.isTrigger = true;

        mySprite = GetComponent<SpriteRenderer>();
        if (mySprite == null) mySprite = GetComponentInChildren<SpriteRenderer>(true);
    }

    void Start()
    {
        // Si ya está desbloqueado globalmente, ocultamos este interactuable
        if (CrystalProgress.Instance != null && CrystalProgress.Instance.crystal3Unlocked)
        {
            if (oneTime)
                gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = true;
            playerTransform = other.transform;
            playerMovement = other.GetComponent<PlayerMovement>();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
            playerInRange = false;
    }

    void Update()
    {
        if (used && oneTime) return;
        if (!playerInRange) return;
        if (DialogueManager.IsAnyDialogueActive) return;

        if (Input.GetKeyDown(interactKey))
        {
            used = true;

            // ✅ Ocultar INMEDIATAMENTE al pulsar E (sin matar la coroutine)
            if (hideWorldRelicImmediatelyOnInteract)
                HideWorldRelicVisualAndInteraction();

            StartCoroutine(UnlockRoutine());
        }
    }

    void HideWorldRelicVisualAndInteraction()
    {
        // Si asignaste un root, lo ocultamos (pero OJO: no debe ser el mismo GO con este script)
        if (worldRelicVisualRoot != null && worldRelicVisualRoot != gameObject)
        {
            worldRelicVisualRoot.SetActive(false);
            return;
        }

        // Si no, ocultamos SpriteRenderer + Collider del propio objeto para que "desaparezca"
        if (mySprite != null) mySprite.enabled = false;
        if (myTriggerCollider != null) myTriggerCollider.enabled = false;
    }

    IEnumerator UnlockRoutine()
    {
        // 1) Diálogo (si existe)
        if (dialogueBeforeUnlock != null && DialogueManager.Instance != null)
        {
            bool finished = false;
            DialogueManager.Instance.StartDialogue(dialogueBeforeUnlock, () => finished = true);
            while (!finished) yield return null;
        }

        // 2) Activar / desactivar colliders tras el diálogo
        if (colliderToDisable != null)
            colliderToDisable.enabled = false;

        if (colliderToEnable != null)
        {
            // Si el GO estaba desactivado, actívalo (por si lo ocultabas entero)
            if (!colliderToEnable.gameObject.activeSelf)
                colliderToEnable.gameObject.SetActive(true);

            colliderToEnable.enabled = true;

            // ✅ Para que NO lo atravieses
            if (enableColliderAsSolid)
                colliderToEnable.isTrigger = false;
        }

        // 3) Congelar movimiento del player
        if (freezePlayerDuringEffect && playerMovement != null)
            playerMovement.enabled = false;

        // 4) Ejecutar animación de la reliquia encima del player
        float totalEffectTime = 0f;

        if (relicEffectObject != null && playerTransform != null)
        {
            relicEffectObject.transform.position = playerTransform.position + relicOffset;

            if (!relicEffectObject.activeSelf)
                relicEffectObject.SetActive(true);

            var effect = relicEffectObject.GetComponent<MagicSpriteEffect>();
            if (effect != null)
            {
                effect.TriggerEffect();
                totalEffectTime =
                    effect.fadeInDuration +
                    effect.riseDuration +
                    effect.glowDuration +
                    effect.fadeOutDuration;
            }
        }

        if (totalEffectTime > 0f)
            yield return new WaitForSeconds(totalEffectTime);

        // 5) Descongelar movimiento
        if (freezePlayerDuringEffect && playerMovement != null)
            playerMovement.enabled = true;

        // 6) Guardar progreso cristal 3
        CrystalProgress.Instance?.UnlockCrystal3();

        // 7) HUD
        if (CrystalHUD.Instance != null)
        {
            CrystalHUD.Instance.RefreshAll();
            CrystalHUD.Instance.FadeInCrystal3();
        }

        // Si quieres que el interactuable desaparezca del todo al final:
        if (oneTime)
            gameObject.SetActive(false);
    }
}