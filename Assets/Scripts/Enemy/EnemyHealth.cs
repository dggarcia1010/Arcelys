using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 5;
    public int currentHealth;

    [Header("Feedback de daño")]
    public Color hitFlashColor = new Color(1f, 0.4f, 0.4f);
    public float hitFlashTime = 0.1f;

    private SpriteRenderer sr;
    private Color originalColor;
    private bool flashing = false;

    private EnemySlow slowRef;

    void Awake()
    {
        currentHealth = maxHealth;

        sr = GetComponent<SpriteRenderer>();
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null) originalColor = sr.color;

        slowRef = GetComponent<EnemySlow>();
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        // feedback visual de golpe
        if (sr != null && !flashing)
        {
            StartCoroutine(FlashOnHit());
        }
    }

    IEnumerator FlashOnHit()
    {
        flashing = true;
        sr.color = hitFlashColor;
        yield return new WaitForSeconds(hitFlashTime);

        if (slowRef != null)
        {
            sr.color = slowRef.GetCurrentDesiredColor();
        }
        else
        {
            sr.color = originalColor;
        }

        flashing = false;
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
