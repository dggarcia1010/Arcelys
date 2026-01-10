using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Referencias")]
    public FollowerEnemy follower;      
    public GameObject colliderPaso;      
    
    [Header("Ajustes")]
    public float detectionRadius = 2.5f; 
    private bool isOpen = false;

    void Update()
    {
        // Solo intentamos abrir si no está abierta y el seguidor ya sigue al jugador
        if (!isOpen && follower != null && follower.IsFollowingPlayer())
        {
            float distance = Vector2.Distance(transform.position, follower.transform.position);

            if (distance <= detectionRadius)
            {
                OpenPath();
            }
        }
    }

    void OpenPath()
    {
        isOpen = true;
        Debug.Log("El seguidor ha abierto el paso.");

        // Desactivamos el objeto que bloquea el camino
        if (colliderPaso != null)
        {
            colliderPaso.SetActive(false); 
        }

        // Opcional: Desactivar visualmente la puerta si este script está en ella
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
