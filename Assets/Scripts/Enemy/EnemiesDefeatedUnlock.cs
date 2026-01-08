using UnityEngine;

public class EnemiesDefeatedUnlock : MonoBehaviour
{
    [Header("Qué cuenta como enemigo")]
    [Tooltip("Tag de los enemigos a contar. Ej: 'Enemy' o 'Boss'. Deja vacío para contar por componente Enemy.")]
    public string enemyTagToCount = "";

    [Header("Puerta / Barrera")]
    public Collider2D colliderToDisable;
    public bool disableObject = true; // true = se apaga el GameObject, false = solo isTrigger

    [Header("Debug")]
    public bool log = true;

    private int enemiesAlive = 0;
    private bool unlocked = false;

    void Start()
    {
        // Cuenta enemigos al iniciar escena
        if (!string.IsNullOrEmpty(enemyTagToCount))
        {
            enemiesAlive = GameObject.FindGameObjectsWithTag(enemyTagToCount).Length;
        }
        else
        {
            enemiesAlive = FindObjectsOfType<Enemy>().Length;
        }

        if (log) Debug.Log($"[EnemiesDefeatedUnlock] Enemigos vivos al inicio: {enemiesAlive}");
        TryUnlock();
    }

    // Llamar esto cuando muera un enemigo
    public void NotifyEnemyDied(GameObject enemyGO)
    {
        if (unlocked) return;

        enemiesAlive = Mathf.Max(0, enemiesAlive - 1);
        if (log) Debug.Log($"[EnemiesDefeatedUnlock] Muere '{enemyGO.name}'. Quedan: {enemiesAlive}");

        TryUnlock();
    }

    private void TryUnlock()
    {
        if (unlocked) return;
        if (enemiesAlive > 0) return;

        unlocked = true;

        if (log) Debug.Log("[EnemiesDefeatedUnlock] ✅ Todos derrotados. Desbloqueando...");

        if (colliderToDisable != null)
        {
            if (disableObject)
                colliderToDisable.gameObject.SetActive(false);
            else
                colliderToDisable.isTrigger = true;
        }
        else
        {
            if (log) Debug.LogWarning("[EnemiesDefeatedUnlock] No hay colliderToDisable asignado.");
        }
    }
}