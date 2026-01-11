using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class NPCDialoguePuzzle : MonoBehaviour
{
    [Header("Datos de diálogo")]
    public Dialogue dialogue;          // frases de este NPC

    [Header("Puzzle")]
    public string npcName = "Aldeano";
    public bool isCorrectNPC = false;  // SOLO true en el NPC final

    [Header("Entrada")]
    public KeyCode interactKey = KeyCode.E;

    [Header("Collider opcional al terminar el diálogo")]
    [Tooltip("Collider que se modificará cuando termine el diálogo de este NPC. Si está vacío, no hace nada.")]
    public Collider2D colliderToAffect;

    [Tooltip("Si está activado, el collider se pondrá en isTrigger. Si está desactivado, se desactivará el GameObject.")]
    public bool setAsTriggerInsteadOfDisable = false;

    [Header("Opciones extra")]
    [Tooltip("Si está activo, el NPC desaparecerá al terminar el diálogo")]
    public bool hideSelfAfterDialogue = false; 

    bool playerInRange = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            // aquí podrías mostrar "Pulsa E para hablar"
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            // ocultar mensaje "Pulsa E para hablar"
        }
    }

    void Update()
    {
        if (!playerInRange) return;

        // Si ya hay un diálogo en pantalla, no dejamos iniciar otro
        if (DialogueManager.IsAnyDialogueActive)
            return;

        // Si el puzzle manager dice que NO se puede hablar con este NPC
        if (VillagePuzzleManager.Instance != null &&
            !VillagePuzzleManager.Instance.CanTalkToNPC(this))
        {
            return;
        }

        if (Input.GetKeyDown(interactKey))
        {
            if (dialogue != null && DialogueManager.Instance != null)
            {
                // Pasamos callback para cuando termine el diálogo
                DialogueManager.Instance.StartDialogue(dialogue, OnDialogueFinished);
            }
            else
            {
                // Si no hay diálogo, aun así llamamos al final
                OnDialogueFinished();
            }

            // Notificamos al manager que hemos hablado con este
            if (VillagePuzzleManager.Instance != null)
            {
                VillagePuzzleManager.Instance.TalkTo(this);
            }
        }
    }

    // Se llama cuando termina el ÚLTIMO cuadro de este diálogo
    void OnDialogueFinished()
    {
        if (colliderToAffect != null)
        {
            if (setAsTriggerInsteadOfDisable)
            {
                colliderToAffect.isTrigger = true;
                Debug.Log("NPCDialoguePuzzle: collider puesto como trigger -> " + colliderToAffect.name);
            }
            else
            {
                colliderToAffect.gameObject.SetActive(false);
                Debug.Log("NPCDialoguePuzzle: collider desactivado -> " + colliderToAffect.name);
            }
        }

        if (hideSelfAfterDialogue)
        {
            gameObject.SetActive(false);
        }
    }
}