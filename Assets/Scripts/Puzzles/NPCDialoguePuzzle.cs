using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class NPCDialoguePuzzle : MonoBehaviour
{
    [Header("Datos de diálogo")]
    public Dialogue dialogue;          // frases de este NPC

    [Header("Puzzle")]
    public string npcName = "Aldeano";
    public bool isCorrectNPC = false;  // SOLO true en el del sombrero

    [Header("Entrada")]
    public KeyCode interactKey = KeyCode.E;

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

        if (DialogueManager.IsAnyDialogueActive)
            return;

        if (Input.GetKeyDown(interactKey))
        {
            if (dialogue != null && DialogueManager.Instance != null)
            {
                DialogueManager.Instance.StartDialogue(dialogue);
            }

            if (VillagePuzzleManager.Instance != null)
            {
                VillagePuzzleManager.Instance.TalkTo(this);
            }
        }
    }
}
