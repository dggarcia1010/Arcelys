using UnityEngine;
using System.Collections;

public class TeleportZone2D : MonoBehaviour
{
    [Header("Destino")]
    public Transform targetPosition;

    [Header("Fade")]
    public float fadeOutTime = 0.35f;
    public float fadeInTime = 0.35f;

    [Header("Opciones")]
    public bool oneTime = false;

    private bool used = false;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (used) return;
        if (!other.CompareTag("Player")) return;
        if (targetPosition == null) return;

        if (ScreenFader.Instance == null)
        {
            Debug.LogWarning("No hay ScreenFader en escena. Crea un Canvas con CanvasGroup + ScreenFader.");
            return;
        }

        StartCoroutine(TeleportWithFade(other));
    }

    private IEnumerator TeleportWithFade(Collider2D player)
    {
        used = true;

        // Desactivar movimiento para que no meta input durante el teleport
        var pm = player.GetComponent<PlayerMovement>();
        if (pm != null) pm.enabled = false;

        // Parar velocidad
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        // Fade a negro
        yield return ScreenFader.Instance.FadeTo(1f, fadeOutTime);

        // Teleport
        player.transform.position = targetPosition.position;

        // 1 frame por seguridad (evita triggers raros)
        yield return null;

        // Fade desde negro
        yield return ScreenFader.Instance.FadeTo(0f, fadeInTime);

        // Reactivar movimiento
        if (pm != null) pm.enabled = true;

        if (!oneTime) used = false;
    }
}