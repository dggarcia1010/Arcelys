using UnityEngine;

public class LadderPuzzleManager : MonoBehaviour
{
    [Header("Zonas objetivo del puzzle")]
    public CrateGoal[] goals;

    [Header("Escalera a activar")]
    public GameObject ladder;

    [Header("Collider que bloquea el paso antes de resolver el puzzle")]
    public Collider2D blocker;

    [Header("Diálogo al completar el puzzle")]
    public Dialogue puzzleCompleteDialogue;   // <-- NUEVO

    private bool puzzleCompleted = false;

    void Start()
    {
        if (ladder != null)
            ladder.SetActive(false);

        if (blocker != null)
            blocker.enabled = true;
    }

    public void OnGoalStateChanged()
    {
        if (puzzleCompleted) return;

        if (AllGoalsSatisfied())
        {
            puzzleCompleted = true;

            if (ladder != null)
                ladder.SetActive(true);

            if (blocker != null)
                blocker.enabled = false;

            // --- NUEVO: disparar diálogo al completar ---
            TryStartCompletionDialogue();
        }
    }

    void TryStartCompletionDialogue()
    {
        if (puzzleCompleteDialogue == null) return;
        if (DialogueManager.Instance == null) return;

        // Si ya hay un diálogo activo, no lo lanzamos (respeta tu sistema)
        if (DialogueManager.IsAnyDialogueActive) return;

        DialogueManager.Instance.StartDialogue(puzzleCompleteDialogue);
    }

    bool AllGoalsSatisfied()
    {
        if (goals == null || goals.Length == 0)
            return false;

        foreach (var g in goals)
        {
            if (g == null) continue;
            if (!g.IsSatisfied) return false;
        }
        return true;
    }
}