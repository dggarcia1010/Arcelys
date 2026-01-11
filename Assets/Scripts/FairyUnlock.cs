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
    public InstructionPanel instructionPanel;

    [Header("Diálogo del hada")]
    public Dialogue fairyDialogue;

    [Header("Estado")]
    public bool unlocked = false;
    public bool oneTime = true;

    private bool isUnlocking = false;

    private Collider2D triggerCol;

    // ✅ Referencia al script que copia animación
    private FairyCopyPlayerAnim fairyCopy;

    void Awake()
    {
        triggerCol = GetComponent<Collider2D>();
        triggerCol.isTrigger = true;

        if (followFairy == null) followFairy = GetComponentInParent<FollowFairy>();

        // ✅ Buscar FairyCopyPlayerAnim en el padre o hijos del hada
        if (followFairy != null)
            fairyCopy = followFairy.GetComponentInChildren<FairyCopyPlayerAnim>(true);
        else
            fairyCopy = GetComponentInParent<FairyCopyPlayerAnim>();

        // ✅ Al inicio NO copia, y fuerza idle down
        if (fairyCopy != null)
            fairyCopy.SetCopyEnabled(false);

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

    void StartUnlockSequence()
    {
        isUnlocking = true;

        if (interactText != null)
            interactText.SetActive(false);

        if (fairyDialogue != null && DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(fairyDialogue, AfterDialogue);
        }
        else
        {
            AfterDialogue();
        }
    }

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

        if (instructionPanel != null)
        {
            instructionPanel.Show(null, 0f);
        }

        if (fairyCopy != null)
            fairyCopy.SetCopyEnabled(true);

        if (oneTime) Destroy(this);

        Debug.Log("Magia desbloqueada, diálogo del hada mostrado y panel de instrucciones abierto.");
    }
}