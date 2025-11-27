using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FairyUnlock : MonoBehaviour
{
    [Header("Asignaciones (opcionales, se autocompletan)")]
    public FollowFairy followFairy;
    public PlayerSpells playerSpells;
    public string playerTag = "Player";
    public KeyCode interactKey = KeyCode.E;

    [Header("UI Mensaje")]
    public GameObject interactText; // tooltip encima del hada

    [Header("Panel de instrucciones")]
    public InstructionPanel instructionPanel; // <-- arrástralo desde el Canvas

    [Header("Diálogo del hada")]
    public Dialogue fairyDialogue; // <-- diálogo que se mostrará antes del panel

    [Header("Estado")]
    public bool unlocked = false;     // ya se ha desbloqueado todo
    public bool oneTime = true;       // solo se puede hacer una vez

    private bool isUnlocking = false; // se está ejecutando la secuencia (dialogo + unlock)

    Collider2D triggerCol;

    void Awake()
    {
        triggerCol = GetComponent<Collider2D>();
        triggerCol.isTrigger = true;

        if (followFairy == null) followFairy = GetComponentInParent<FollowFairy>();

        if (playerSpells == null)
        {
            var p = GameObject.FindGameObjectWithTag(playerTag);
            if (p) playerSpells = p.GetComponent<PlayerSpells>();
        }

        if (interactText != null)
            interactText.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!unlocked && !isUnlocking && other.CompareTag(playerTag) && interactText != null)
            interactText.SetActive(true);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (unlocked || isUnlocking) return;
        if (!other.CompareTag(playerTag)) return;

        if (Input.GetKeyDown(interactKey))
        {
            StartUnlockSequence();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) && interactText != null)
            interactText.SetActive(false);
    }

    /// <summary>
    /// Empieza la secuencia: primero diálogo, luego desbloquear y mostrar panel.
    /// </summary>
    void StartUnlockSequence()
    {
        isUnlocking = true;

        if (interactText != null)
            interactText.SetActive(false);

        // Si hay diálogo y DialogueManager, lo mostramos primero
        if (fairyDialogue != null && DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(fairyDialogue, AfterDialogue);
        }
        else
        {
            // Si no hay diálogo, vamos directos al desbloqueo
            AfterDialogue();
        }
    }

    /// <summary>
    /// Lógica que antes estaba en Unlock: se ejecuta al terminar el diálogo.
    /// </summary>
    void AfterDialogue()
    {
        unlocked = true;

        if (playerSpells != null) 
            playerSpells.UnlockMagic();

        if (followFairy != null)
        {
            followFairy.SetFairyActive(true);
            var phys = followFairy.GetComponent<CircleCollider2D>();
            if (phys != null) phys.isTrigger = true;
        }

        // Mostrar panel con instrucciones
        if (instructionPanel != null)
        {
            instructionPanel.Show(
                "Pulsa 1 (Viento), 2 (Hielo), 3 (Fuego), 4 (Luz) para seleccionar el hechizo.\nPulsa ESPACIO para lanzarlo hacia el ratón.",
                0f // 0 = no autocierra
            );
        }

        if (oneTime) Destroy(this);

        Debug.Log("Magia desbloqueada, diálogo del hada mostrado y panel de instrucciones abierto.");
    }
}