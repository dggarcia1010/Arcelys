using UnityEngine;

public class LadderPuzzleManager : MonoBehaviour
{
    [Header("Zonas objetivo del puzzle")]
    public CrateGoal[] goals;

    [Header("Escalera a activar")]
    public GameObject ladder;

    [Header("Collider que bloquea el paso antes de resolver el puzzle")]
    public Collider2D blocker;  // ← NUEVO

    private bool puzzleCompleted = false;

    void Start()
    {
        if (ladder != null)
            ladder.SetActive(false);

        // El collider debe estar activo mientras el puzzle no esté resuelto
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
                blocker.enabled = false; // ← DESACTIVAR BLOQUEO
        }
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