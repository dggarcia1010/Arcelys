using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{
    [Header("Configuración de la cutscene")]
    public Transform cutsceneFocusPoint;   // Empty donde enfocar la cámara
    public float duration = 4f;            // Segundos que dura

    private bool hasPlayed = false;        // Solo local (se resetea al reload escena)

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Salir rápido si no es jugador o ya se reprodujo
        if (!other.CompareTag("Player") || hasPlayed)
            return;

        hasPlayed = true;
        Debug.Log("Cutscene iniciada por primera vez");

        if (CutsceneManager.Instance != null)
        {
            CutsceneManager.Instance.PlayCutscene(
                cutsceneFocusPoint,
                duration,
                () => Debug.Log("Cutscene terminada")
            );
        }

        // Desactivamos el collider para que nunca más detecte al jugador
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
            Debug.Log("Trigger desactivado: nunca más se activará aunque pases otra vez");
        }

        Destroy(gameObject);
    }
}
