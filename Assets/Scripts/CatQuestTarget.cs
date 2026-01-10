using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CatQuestTarget : MonoBehaviour
{
    [Header("Diálogo al rescatar al gato")]
    public Dialogue catRescueDialogue;   // Lo que "dice" el gato o el prota al rescatarlo

    [Header("Entrada")]
    public KeyCode interactKey = KeyCode.E;
    public string playerTag = "Player";

    private bool playerInRange = false;
    private bool alreadyRescued = false;

    void Awake()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void Start()
    {
        // Por si se entra en la escena después de haberlo rescatado ya
        var quest = CatQuestManager.Instance;
        if (quest != null && quest.catRescued)
        {
            alreadyRescued = true;
            gameObject.SetActive(false); // ocultar gato si ya se rescató
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = true;
            // TODO: mostrar "Pulsa E para rescatar al gato"
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
            // TODO: ocultar mensaje
        }
    }

    void Update()
    {
        if (!playerInRange || alreadyRescued) return;

        if (DialogueManager.IsAnyDialogueActive)
            return;

        if (Input.GetKeyDown(interactKey))
        {
            RescueCat();
        }
    }

    void RescueCat()
    {
        var quest = CatQuestManager.Instance;

        if (quest != null)
        {
            quest.CompleteMission();
        }

        alreadyRescued = true;

        if (DialogueManager.Instance != null && catRescueDialogue != null)
        {
            // Al acabar el diálogo, quitamos al gato (o lo desactivamos)
            DialogueManager.Instance.StartDialogue(catRescueDialogue, () =>
            {
                gameObject.SetActive(false);
            });
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}