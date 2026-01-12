using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraMovement : MonoBehaviour
{
    public Transform target; // Jugador normalmente
    public float smoothing = 5f;
    public Tilemap map; // Para límites
    
    // Soporte para cutscenes
    public Transform cutsceneTarget; // Target temporal (empty) (opcional)
    public bool isInCutscene = false;
    
    private Vector2 minPosition;
    private Vector2 maxPosition;
    private Vector3? cutsceneStartPosition = null; // Para guardar posición inicial de la cutscene

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

        Transform current = isInCutscene
            ? (cutsceneTarget != null ? cutsceneTarget : target)
            : target;
            
        if (current == null) return;

        float desiredX;
        float desiredY;
        
        if (isInCutscene)
        {
            // ✅ En cutscene: sigue al focusPoint en X e Y
            desiredX = current.position.x;
            desiredY = current.position.y;
            
            // Solo sigue al jugador en Y si no hay cutsceneTarget
            if (cutsceneTarget == null)
            {
                // Si no hay focus point específico, seguimos al jugador en Y
                desiredY = target.position.y;
            }
        }
        else
        {
            desiredX = current.position.x;
            desiredY = current.position.y;
        }

        Vector3 targetPos = new Vector3(desiredX, desiredY, transform.position.z);

        // Clamp solo aplica cuando no está en cutscene
        if (!isInCutscene)
        {
            targetPos.x = Mathf.Clamp(targetPos.x, minPosition.x, maxPosition.x);
            targetPos.y = Mathf.Clamp(targetPos.y, minPosition.y, maxPosition.y);
        }
        else if (map != null) // En cutscene también hacemos clamp para no salir del mapa
        {
            targetPos.x = Mathf.Clamp(targetPos.x, minPosition.x, maxPosition.x);
            targetPos.y = Mathf.Clamp(targetPos.y, minPosition.y, maxPosition.y);
        }

        float delta = isInCutscene ? Time.unscaledDeltaTime : Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothing * delta);
    }

    public void StartCutscene(Transform newTarget = null)
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
