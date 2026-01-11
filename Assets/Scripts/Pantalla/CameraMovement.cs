using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraMovement : MonoBehaviour
{
    public Transform target;                // Jugador normalmente
    public float smoothing = 5f;
    public Tilemap map;                     // Para límites

    // Soporte para cutscenes
    public Transform cutsceneTarget;        // Target temporal (empty)
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
        Transform current = isInCutscene ? cutsceneTarget : target;
        if (current == null) return;

        Vector3 targetPos = new Vector3(current.position.x, current.position.y, transform.position.z);

        targetPos.x = Mathf.Clamp(targetPos.x, minPosition.x, maxPosition.x);
        targetPos.y = Mathf.Clamp(targetPos.y, minPosition.y, maxPosition.y);

        float delta = isInCutscene ? Time.unscaledDeltaTime : Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothing * delta);
    }

    public void StartCutscene(Transform newTarget)
    {
        cutsceneTarget = newTarget;
        isInCutscene = true;
    }

    public void EndCutscene()
    {
        isInCutscene = false;
        cutsceneTarget = null;
    }
}
