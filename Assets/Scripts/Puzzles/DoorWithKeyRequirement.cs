using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class FollowerDoorOpener : MonoBehaviour
{
    [Header("Requisito del Follower (la llave)")]
    public FollowerEnemy followerEnemy; // Asigna tu objeto con FollowerEnemy aquí (la llave flotante)

    [Header("Collider de la puerta a abrir")]
    public Collider2D doorBlockerCollider; // Collider físico que bloquea el paso
    public bool makeTriggerInsteadOfDisable = true; // Recomendado: true

    [Header("Objeto visual de la puerta")]
    public GameObject doorVisualObject; // Asigna aquí el GameObject llamado "Door" que quieres que desaparezca

    [Header("Interacción")]
    public KeyCode interactKey = KeyCode.E;

    [Header("Tiempos del Fade y efecto")]
    public float fadeDuration = 0.5f;     // Tiempo que tarda en ir a negro y volver
    public float blackScreenTime = 1f;    // Tiempo que permanece negro
    public float orbitPreviewTime = 1.2f; // Tiempo que dejamos ver la órbita antes del fade (ajustable)

    private bool playerInRange = false;
    private bool doorAlreadyOpened = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Jugador cerca de la puerta. Pulsa E para intentar abrirla.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void Update()
    {
        if (!playerInRange || doorAlreadyOpened) return;

        if (Input.GetKeyDown(interactKey))
        {
            AttemptToOpenDoor();
        }
    }

    private void AttemptToOpenDoor()
    {
        if (followerEnemy == null)
        {
            Debug.LogError("FollowerDoorOpener: Asigna el FollowerEnemy (llave) en el Inspector.");
            return;
        }

        if (doorBlockerCollider == null)
        {
            Debug.LogError("FollowerDoorOpener: Asigna el collider bloqueante de la puerta.");
            return;
        }

        if (!followerEnemy.IsFollowingPlayer())
        {
            Debug.Log("La llave aún no te sigue. No puedes abrir la puerta todavía.");
            return;
        }

        // ¡Condición cumplida! Iniciamos la secuencia mágica
        doorAlreadyOpened = true;
        StartCoroutine(OpenDoorWithFadeAndOrbit());
    }

    private IEnumerator OpenDoorWithFadeAndOrbit()
    {
        if (ScreenFader.Instance == null)
        {
            Debug.LogError("No se encontró ScreenFader en la escena.");
            yield break;
        }

        Debug.Log("Secuencia mágica iniciada...");

        // 1. Activamos el modo órbita mágica en la llave
        Transform orbitCenter = doorVisualObject != null 
            ? doorVisualObject.transform 
            : GameObject.Find("Door")?.transform;

        if (orbitCenter != null)
        {
            followerEnemy.StartMagicOrbit(orbitCenter);
            Debug.Log("Llave ahora orbita alrededor de la puerta como un hada");
        }
        else
        {
            Debug.LogWarning("No se encontró centro de órbita (Door o doorVisualObject)");
        }

        // 2. Dejamos ver la órbita un momento (para que se aprecie el efecto)
        yield return new WaitForSeconds(orbitPreviewTime);

        // 3. Fade a negro
        yield return ScreenFader.Instance.FadeTo(1f, fadeDuration);

        // 4. Mantener negro
        yield return new WaitForSecondsRealtime(blackScreenTime);

        // 5. Durante el negro: desaparecemos el objeto visual de la puerta
        if (doorVisualObject != null)
        {
            doorVisualObject.SetActive(false);
            Debug.Log("Objeto visual de la puerta desactivado.");
        }
        else
        {
            GameObject door = GameObject.Find("Door");
            if (door != null)
            {
                door.SetActive(false);
                Debug.Log("Objeto 'Door' encontrado y desactivado.");
            }
            else
            {
                Debug.LogWarning("No se encontró ningún GameObject llamado 'Door'.");
            }
        }

        // 6. Abrir el paso físico
        if (makeTriggerInsteadOfDisable)
        {
            doorBlockerCollider.isTrigger = true;
        }
        else
        {
            doorBlockerCollider.gameObject.SetActive(false);
        }

        // 7. Fade out a transparente
        yield return ScreenFader.Instance.FadeTo(0f, fadeDuration);

        Debug.Log("Transición completada. Puerta abierta.");

        // Desactivamos la zona de interacción
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }
}
