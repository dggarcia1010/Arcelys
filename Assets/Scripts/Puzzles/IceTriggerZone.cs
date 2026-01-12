using UnityEngine;

public class IceActivator : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject iceOverlay;         // ← Arrastra aquí el GameObject "IceOverlay"
    [SerializeField] private GameObject colliderToDisable;  // ← La barrera/puerta/colisionador que quieres quitar

    private bool isActivated = false; // Evita activar varias veces

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Solo nos interesa el proyectil del jugador
        if (other.CompareTag("Projectile") == false)
            return;

        if (!other.TryGetComponent<Projectile2D>(out var projectile))
            return;

        // Tiene que ser del jugador y de tipo hielo
        if (projectile.team != Projectile2D.Team.Player || !projectile.isIce)
            return;

        if (isActivated)
            return;

        isActivated = true;

        // 1. Mostrar la capa de hielo
        if (iceOverlay != null)
        {
            iceOverlay.SetActive(true);
            Debug.Log("[ICE] Capa de escarcha activada");
        }

        // 2. Desactivar el collider/bloqueo
        if (colliderToDisable != null)
        {
            colliderToDisable.SetActive(false);
            Debug.Log("[ICE] Barrera/colisionador desactivado");
        }

        // Feedback visual opcional: destruir el proyectil inmediatamente
        Destroy(other.gameObject);
    }
}
