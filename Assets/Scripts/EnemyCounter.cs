using UnityEngine;

public class EnemyCounter2D : MonoBehaviour
{
    [SerializeField]
    private string colliderObjectName = "ColliderPaso";

    [SerializeField]
    private string enemyTag = "Enemy";

    private Collider2D targetCollider;
    private int lastCount = -1;

    void Start()
    {
        FindCollider();
    }

    void Update()
    {
        int current = CountEnemies();

        if (current != lastCount)
        {
            lastCount = current;

            if (targetCollider == null)
                FindCollider();

            if (targetCollider != null)
            {
                targetCollider.enabled = (current > 0);
                // Debug.Log($"Enemigos restantes: {current} → Collider activado: {targetCollider.enabled}");
            }
        }
    }

    private void FindCollider()
    {
        GameObject go = GameObject.Find(colliderObjectName);

        if (go != null)
        {
            targetCollider = go.GetComponent<Collider2D>();

            if (targetCollider != null)
            {
                Debug.Log($"¡Collider 2D encontrado en '{colliderObjectName}'!");
            }
            else
            {
                Debug.LogError($"'{colliderObjectName}' existe pero NO tiene Collider2D");
            }
        }
        else
        {
            Debug.LogWarning($"No se encontró GameObject llamado '{colliderObjectName}'");
        }
    }

    private int CountEnemies()
    {
        return GameObject.FindGameObjectsWithTag(enemyTag).Length;
    }
}
