using UnityEngine;

public class LadderPuzzleManager : MonoBehaviour
{
    [Header("Zonas objetivo del puzzle")]
    public CrateGoal[] goals;

    [Header("Escalera a activar")]
    public GameObject ladder;

    private bool puzzleCompleted = false;

    void Start()
    {
        if (ladder != null)
            ladder.SetActive(false);
    }

    public void OnGoalStateChanged()
    {
        if (puzzleCompleted) return;

        if (AllGoalsSatisfied())
        {
            puzzleCompleted = true;
            if (ladder != null)
                ladder.SetActive(true);
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