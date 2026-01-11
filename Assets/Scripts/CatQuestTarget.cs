using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class CatQuestTarget : MonoBehaviour
{
    [Header("Diálogo al rescatar al gato")]
    public Dialogue catRescueDialogue;

    [Header("Configuración del Árbol")]
    public GameObject treeObject;
    public Animator treeAnimator;

    [Header("Al terminar el diálogo (nuevo)")]
    public GameObject objectToShowAfterDialogue;   // <-- NUEVO
    public bool showOnlyOnce = true;               // <-- opcional
    private static bool objectShownOnce = false;   // <-- opcional para persistencia simple

    [Header("Entrada")]
    public KeyCode interactKey = KeyCode.E;
    public string playerTag = "Player";

    [Header("Tiempos")]
    public float animationDuration = 1f;
    public float postAnimationDelay = 0.3f;

    private bool playerInRange = false;
    private bool alreadyRescued = false;

    // Variable para controlar persistencia entre escenas
    private static bool treeAnimationPlayed = false;
    private static bool treeAnimatorDisabled = false;

    void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;

        // Si quieres que empiece oculto por defecto:
        if (objectToShowAfterDialogue != null)
        {
            if (showOnlyOnce && objectShownOnce)
                objectToShowAfterDialogue.SetActive(true);
            else
                objectToShowAfterDialogue.SetActive(false);
        }

        if (treeObject != null)
        {
            // Si ya se mostró antes, desactivar Animator inmediatamente
            if (treeAnimationPlayed)
            {
                treeObject.SetActive(true);
                if (treeAnimator != null)
                {
                    treeAnimator.enabled = false; // Desactivar Animator
                    treeAnimatorDisabled = true;
                }
            }
            else
            {
                // Primera vez: ocultar árbol
                treeObject.SetActive(false);
            }
        }
    }

    void Start()
    {
        var quest = CatQuestManager.Instance;
        if (quest != null && quest.catRescued)
        {
            alreadyRescued = true;

            // Si ya se rescató, mostrar árbol con Animator desactivado
            if (treeObject != null)
            {
                treeObject.SetActive(true);
                treeAnimationPlayed = true;

                if (treeAnimator != null)
                {
                    treeAnimator.enabled = false;
                    treeAnimatorDisabled = true;
                }
            }
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
        }
    }

    void Update()
    {
        if (!playerInRange || alreadyRescued) return;
        if (DialogueManager.IsAnyDialogueActive) return;

        if (Input.GetKeyDown(interactKey))
        {
            StartCatRescueSequence();
        }
    }

    void StartCatRescueSequence()
    {
        alreadyRescued = true;

        // Completar misión
        var quest = CatQuestManager.Instance;
        if (quest != null)
        {
            quest.CompleteMission();
        }

        // Iniciar secuencia
        StartCoroutine(RescueSequenceRoutine());
    }

    IEnumerator RescueSequenceRoutine()
    {
        // PASO 1: Mostrar árbol con animación (UNA SOLA VEZ)
        yield return StartCoroutine(PlayTreeAnimationOnce());

        // PASO 2: Desactivar Animator para siempre
        DisableTreeAnimatorPermanently();

        // PASO 3: Pequeño delay
        yield return new WaitForSeconds(postAnimationDelay);

        // PASO 4: Iniciar diálogo
        StartDialogue();
    }

    IEnumerator PlayTreeAnimationOnce()
    {
        // Si ya se reprodujo antes, salir
        if (treeAnimationPlayed || treeObject == null)
        {
            yield break;
        }

        // Marcar como reproducida (para siempre)
        treeAnimationPlayed = true;

        // Activar árbol
        treeObject.SetActive(true);

        // Reproducir animación si hay Animator
        if (treeAnimator != null && !treeAnimatorDisabled)
        {
            // Activar Animator temporalmente
            treeAnimator.enabled = true;
            treeAnimator.Rebind();

            // Reproducir animación
            treeAnimator.Play("Appear", 0, 0f);

            // Esperar a que termine la animación
            float animLength = GetAnimationLength("Appear");
            yield return new WaitForSeconds(animLength);
        }
        else
        {
            // Animación alternativa por código
            yield return StartCoroutine(DefaultScaleAnimation());
        }
    }

    void DisableTreeAnimatorPermanently()
    {
        if (treeAnimator != null)
        {
            // Desactivar el Animator para siempre
            treeAnimator.enabled = false;
            treeAnimatorDisabled = true;

            Debug.Log("Animator del árbol desactivado permanentemente");
        }
    }

    IEnumerator DefaultScaleAnimation()
    {
        if (treeObject == null) yield break;

        Vector3 originalScale = treeObject.transform.localScale;
        treeObject.transform.localScale = Vector3.zero;

        float time = 0f;

        while (time < animationDuration)
        {
            time += Time.deltaTime;
            float t = time / animationDuration;

            // Curva ease-out
            t = 1f - Mathf.Pow(1f - t, 3f);

            treeObject.transform.localScale = originalScale * t;
            yield return null;
        }

        treeObject.transform.localScale = originalScale;
    }

    float GetAnimationLength(string animationName)
    {
        if (treeAnimator == null) return animationDuration;

        RuntimeAnimatorController ac = treeAnimator.runtimeAnimatorController;
        if (ac == null) return animationDuration;

        foreach (AnimationClip clip in ac.animationClips)
        {
            if (clip.name == animationName)
            {
                return clip.length;
            }
        }

        return animationDuration;
    }

    void StartDialogue()
    {
        if (DialogueManager.Instance != null && catRescueDialogue != null)
        {
            DialogueManager.Instance.StartDialogue(catRescueDialogue, () =>
            {
                // ✅ Cuando termina el diálogo: mostrar objeto
                if (objectToShowAfterDialogue != null)
                {
                    if (!showOnlyOnce || !objectShownOnce)
                    {
                        objectToShowAfterDialogue.SetActive(true);
                        objectShownOnce = true;
                    }
                }

                // Luego ocultar este target
                gameObject.SetActive(false);
            });
        }
        else
        {
            // Si no hay diálogo, hacemos lo mismo igualmente
            if (objectToShowAfterDialogue != null)
            {
                if (!showOnlyOnce || !objectShownOnce)
                {
                    objectToShowAfterDialogue.SetActive(true);
                    objectShownOnce = true;
                }
            }

            gameObject.SetActive(false);
        }
    }

    // Métodos estáticos para control global
    public static bool HasTreeAnimationPlayed()
    {
        return treeAnimationPlayed;
    }

    public static void ResetTreeAnimation()
    {
        treeAnimationPlayed = false;
        treeAnimatorDisabled = false;
        objectShownOnce = false;
        Debug.Log("Animación del árbol reseteada (solo para testing)");
    }
}