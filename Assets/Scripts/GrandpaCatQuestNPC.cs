using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GrandpaCatQuestNPC : MonoBehaviour
{
    [Header("Diálogos del abuelo")]
    public Dialogue firstTimeDialogue;      // Te da la misión del gato
    public Dialogue duringMissionDialogue;  // Opcional: si hablas con él sin haber rescatado al gato
    public Dialogue afterMissionDialogue;   // Cuando ya has rescatado al gato

    [Header("Entrada")]
    public KeyCode interactKey = KeyCode.E;
    public string playerTag = "Player";

    private bool playerInRange = false;

    void Awake()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = true;
            // TODO: mostrar "Pulsa E para hablar"
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
            // TODO: ocultar "Pulsa E para hablar"
        }
    }

    void Update()
    {
        if (!playerInRange) return;

        // ⬇️ NUEVO: si hay un diálogo abierto, ignorar la E
        if (DialogueManager.IsAnyDialogueActive)
            return;

        if (Input.GetKeyDown(interactKey))
        {
            TalkToPlayer();
        }
    }

    void TalkToPlayer()
    {
        if (DialogueManager.Instance == null)
        {
            Debug.LogWarning("No hay DialogueManager en la escena.");
            return;
        }

        var quest = CatQuestManager.Instance;

        // Si todavía no hay manager por alguna razón
        if (quest == null)
        {
            // Simplemente muestra el diálogo normal
            if (firstTimeDialogue != null)
                DialogueManager.Instance.StartDialogue(firstTimeDialogue);
            return;
        }

        // 1) Todavía no ha aceptado la misión
        if (!quest.missionAccepted)
        {
            if (firstTimeDialogue != null)
            {
                DialogueManager.Instance.StartDialogue(firstTimeDialogue, () =>
                {
                    quest.AcceptMission(); // al terminar el diálogo, se acepta la misión
                });
            }
        }
        // 2) Misión aceptada pero gato NO rescatado aún
        else if (!quest.catRescued)
        {
            if (duringMissionDialogue != null)
            {
                DialogueManager.Instance.StartDialogue(duringMissionDialogue);
            }
        }
        // 3) Gato rescatado → diálogo nuevo
        else
        {
            if (afterMissionDialogue != null)
            {
                DialogueManager.Instance.StartDialogue(afterMissionDialogue);
            }
        }
    }
}