using UnityEngine;

public class FairyMover : MonoBehaviour
{
    [Header("Destino al que se moverá el hada")]
    public Transform target;          // punto al que debe ir
    public float moveSpeed = 2f;      // velocidad de movimiento
    public float stopDistance = 0.05f; // distancia mínima para considerar que ha llegado

    private bool isMoving = false;

    // Llamar a esto desde EnemyGroupDialogueTrigger (o donde quieras)
    public void StartMove()
    {
        if (target == null)
        {
            Debug.LogWarning("FairyMover: no hay target asignado para el hada.");
            return;
        }

        isMoving = true;
    }

    void Update()
    {
        if (!isMoving || target == null) return;

        Vector3 direction = target.position - transform.position;
        float distance = direction.magnitude;

        if (distance <= stopDistance)
        {
            // Ha llegado
            isMoving = false;
            // Aquí podrías lanzar una animación, otro diálogo, etc.
            return;
        }

        Vector3 step = direction.normalized * moveSpeed * Time.deltaTime;
        if (step.magnitude > distance)
            step = direction;

        transform.position += step;
    }
}