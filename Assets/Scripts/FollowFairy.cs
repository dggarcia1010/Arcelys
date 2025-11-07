using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowFairy : MonoBehaviour    
{
    [Header("General")]
    [SerializeField] bool fairyActive = true;       
    [SerializeField] bool hideWhenInactive = true;  

    [Header("Target")]
    [SerializeField] Transform target;              // Player a seguir

    [Header("Offsets")]
    [SerializeField] Vector2 offset = new Vector2(1f, 0.5f); 

    [Header("Movimiento")]
    [SerializeField] float smoothTime = 0.15f;      
    [SerializeField] float bobAmplitude = 0.1f;     
    [SerializeField] float bobFrequency = 3f;       

    Vector3 velocity;
    SpriteRenderer spriteRenderer;

    void Start()
    {
        if (target == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) target = p.transform;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        UpdateActiveState();
}

    void LateUpdate()
    {
        if (!fairyActive || !target) return;

        // Posición base con offset
        Vector3 desired = target.position + (Vector3)offset;

        // Movimiento de flotación vertical
        desired.y += Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;

        // Movimiento suave hacia el objetivo
        transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);
    }

    public void SetFairyActive(bool active)
    {
        fairyActive = active;
        UpdateActiveState();
    }

void UpdateActiveState()
{
    if (spriteRenderer == null) return;

    if (hideWhenInactive)
    {
        spriteRenderer.enabled = fairyActive;
    }
    else
    {
        spriteRenderer.enabled = true;
    }
}

    void OnDrawGizmosSelected()
    {
        if (!target) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, target.position);
        Gizmos.DrawSphere(transform.position, 0.05f);
    }
}