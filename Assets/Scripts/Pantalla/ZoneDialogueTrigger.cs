using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ZoneDialogueTrigger : MonoBehaviour
{
    [Header("Diálogo")]
    public Dialogue dialogue;          // El diálogo que quieres mostrar al pasar

    [Header("Detección")]
    public string playerTag = "Player";

    [Header("Comportamiento")]
    public bool oneTime = true;        // ¿Solo una vez?
    public float delay = 0f;           // Retraso en segundos antes de mostrar el diálogo

    private bool hasTriggered = false;

    void Awake()
    {
        // Aseguramos que el collider sea trigger
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (oneTime && hasTriggered) return;

        // Si ya hay un diálogo activo, no disparamos este
        if (DialogueManager.IsAnyDialogueActive)
            return;

        hasTriggered = true;

        if (delay > 0f)
        {
            StartCoroutine(StartDialogueAfterDelay());
        }
        else
        {
            StartDialogueNow();
        }
    }

    IEnumerator StartDialogueAfterDelay()
    {
        yield return new WaitForSeconds(delay);

        // Por si durante la espera ha empezado otro diálogo
        if (DialogueManager.IsAnyDialogueActive) yield break;

        StartDialogueNow();
    }

    void StartDialogueNow()
    {
        if (DialogueManager.Instance != null && dialogue != null)
        {
            DialogueManager.Instance.StartDialogue(dialogue);
        }
    }
}