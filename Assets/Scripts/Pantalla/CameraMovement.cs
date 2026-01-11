using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraMovement : MonoBehaviour
{
    public Transform target;                // Jugador normalmente
    public float smoothing = 5f;
    public Tilemap map;                     // Para límites

    // Soporte para cutscenes
    public Transform cutsceneTarget;        // Target temporal (empty) (opcional)
    public bool isInCutscene = false;

    private Vector2 minPosition;
    private Vector2 maxPosition;

    void Start()
    {
        if (map != null)
        {
            map.CompressBounds();
            Vector3 minCell = map.CellToWorld(map.cellBounds.min);
            Vector3 maxCell = map.CellToWorld(map.cellBounds.max);

            float camHalfHeight = Camera.main.orthographicSize;
            float camHalfWidth = camHalfHeight * Camera.main.aspect;

            minPosition = new Vector2(minCell.x + camHalfWidth, minCell.y + camHalfHeight);
            maxPosition = new Vector2(maxCell.x - camHalfWidth, maxCell.y - camHalfHeight);
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // En cutscene queremos seguir al player SOLO en Y.
        // Fuera de cutscene, seguimos al target normal en X e Y.
        Transform current = isInCutscene
            ? (cutsceneTarget != null ? cutsceneTarget : target) // si no hay cutsceneTarget, usamos player igual
            : target;

        if (current == null) return;

        // Base de la posición objetivo
        float desiredX;
        float desiredY;

        if (isInCutscene)
        {
            // ✅ Bloquea X: se queda donde está la cámara
            desiredX = transform.position.x;

            // ✅ Sigue Y: normalmente al jugador (target), o al cutsceneTarget si lo usas
            desiredY = current.position.y;
        }
        else
        {
            desiredX = current.position.x;
            desiredY = current.position.y;
        }

        Vector3 targetPos = new Vector3(desiredX, desiredY, transform.position.z);

        // Clamp solo en el eje que corresponda
        // (En cutscene clampleamos Y; X lo dejamos fijo tal cual, pero igualmente podrías clamplearlo por seguridad)
        if (!isInCutscene)
        {
            targetPos.x = Mathf.Clamp(targetPos.x, minPosition.x, maxPosition.x);
        }
        // Y siempre conviene clamplearlo
        targetPos.y = Mathf.Clamp(targetPos.y, minPosition.y, maxPosition.y);

        float delta = isInCutscene ? Time.unscaledDeltaTime : Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothing * delta);
    }

    public void StartCutscene(Transform newTarget = null)
    {
        cutsceneTarget = newTarget; // puedes pasar null si quieres seguir al player en Y
        isInCutscene = true;
    }

    public void EndCutscene()
    {
        isInCutscene = false;
        cutsceneTarget = null;
    }
}