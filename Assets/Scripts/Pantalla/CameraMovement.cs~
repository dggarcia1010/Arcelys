using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraMovement : MonoBehaviour
{
    public Transform target;
    public float smoothing = 5f;
    public Tilemap map; //ground

    private Vector2 minPosition;
    private Vector2 maxPosition;

    void Start()
    {
        if (map == null) return;

        map.CompressBounds();
        Vector3 minCell = map.CellToWorld(map.cellBounds.min);
        Vector3 maxCell = map.CellToWorld(map.cellBounds.max);

        float camHalfHeight = Camera.main.orthographicSize;
        float camHalfWidth = camHalfHeight * Camera.main.aspect;

        minPosition = new Vector2(minCell.x + camHalfWidth, minCell.y + camHalfHeight);
        maxPosition = new Vector2(maxCell.x - camHalfWidth, maxCell.y - camHalfHeight);
    }

    void LateUpdate()
    {
        if (target == null) return;

        if (transform.position != target.position)
        {
            Vector3 targetPosition = new Vector3(
                target.position.x,
                target.position.y,
                transform.position.z
            );

            targetPosition.x = Mathf.Clamp(targetPosition.x, minPosition.x, maxPosition.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minPosition.y, maxPosition.y);

            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition,
                smoothing * Time.deltaTime
            );
        }
    }
}