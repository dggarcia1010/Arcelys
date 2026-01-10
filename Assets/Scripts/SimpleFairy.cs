using UnityEngine;

public class SimpleFairy : MonoBehaviour
{
    [Header("Movimiento del hada")]
    public Transform destino;  
    public float velocidad = 2f;  

    [Header("Flotación")]
    public float bobAmplitude = 0.005f;
    public float bobFrequency = 3f;  

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (destino == null) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            destino.position,
            velocidad * Time.deltaTime
        );

        float bob = Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;

        transform.position = new Vector3(
            transform.position.x,
            transform.position.y + bob,
            transform.position.z
        );

        if (Vector3.Distance(transform.position, destino.position) < 0.05f)
        {
            enabled = false;
        }
    }
}