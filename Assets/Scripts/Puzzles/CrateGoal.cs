using UnityEngine;

public class CrateGoal : MonoBehaviour
{
    [Tooltip("ID de la caja que debe venir aquí (mismo texto que en PuzzleCrate.crateId)")]
    public string requiredCrateId;

    [Tooltip("Manager del puzzle que controla la escalera")]
    public LadderPuzzleManager manager;

    private PuzzleCrate currentCrate;
    public bool IsSatisfied => currentCrate != null;

    void OnTriggerEnter2D(Collider2D other)
    {
        var crate = other.GetComponent<PuzzleCrate>();
        if (crate != null && crate.crateId == requiredCrateId)
        {
            currentCrate = crate;
            if (manager != null)
                manager.OnGoalStateChanged();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (currentCrate != null && other.gameObject == currentCrate.gameObject)
        {
            currentCrate = null;
            if (manager != null)
                manager.OnGoalStateChanged();
        }
    }
}