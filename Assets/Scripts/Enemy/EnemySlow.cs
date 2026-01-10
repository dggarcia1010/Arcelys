using UnityEngine;
using System.Collections;

public class EnemySlow : MonoBehaviour
{
    private Enemy enemyRef;
    private float originalSpeed;
    private Coroutine slowRoutine;

    private SpriteRenderer sr;
    private Color originalColor;

    [Header("Estado")]
    public bool isSlowed = false;

    [Header("Visual congelado")]
    public Color frozenColor = new Color(0.5f, 0.8f, 1f); 

    void Awake()
    {
        enemyRef = GetComponent<Enemy>();
        if (enemyRef == null)
        {
            Debug.LogError("[EnemySlow] No se encontró componente Enemy en " + gameObject.name);
            enabled = false;
            return;
        }

        originalSpeed = enemyRef.speed;

        sr = GetComponent<SpriteRenderer>();
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null) originalColor = sr.color;
    }

    public void ApplySlow(float slowMultiplier, float duration)
    {
        if (slowRoutine != null)
        {
            StopCoroutine(slowRoutine);
        }
        slowRoutine = StartCoroutine(SlowCoroutine(slowMultiplier, duration));
    }

    private IEnumerator SlowCoroutine(float slowMultiplier, float duration)
    {
        isSlowed = true;
        enemyRef.speed = originalSpeed * slowMultiplier;

        if (sr != null)
        {
            sr.color = frozenColor;
        }

        yield return new WaitForSeconds(duration);
        enemyRef.speed = originalSpeed;
        isSlowed = false;

        if (sr != null)
        {
            sr.color = originalColor;
        }

        slowRoutine = null;
    }

    public Color GetCurrentDesiredColor()
    {
        if (isSlowed && sr != null)
        {
            return frozenColor;
        }
        return originalColor;
    }
}