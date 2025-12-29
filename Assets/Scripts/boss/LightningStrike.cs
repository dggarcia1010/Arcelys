using UnityEngine;

public class LightningStrike : MonoBehaviour
{
    public float damage = 3f;
    public float lifeTime = 0.6f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        HealthSystem hs = other.GetComponent<HealthSystem>();
        if (hs != null)
            hs.TakeDamage(damage);
    }
}