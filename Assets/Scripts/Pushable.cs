using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Pushable : MonoBehaviour
{
    [Header("Ventana de empuje")]
    public float unlockTime = 0.35f;
    public bool zeroOutOnRelock = true;

    Rigidbody2D rb;
    Coroutine unlockCo;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Lock();
    }

    public void Lock()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezePositionX |
                         RigidbodyConstraints2D.FreezePositionY |
                         RigidbodyConstraints2D.FreezeRotation;
    }

    public void Unlock()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // permite moverse en X/Y
    }

    public void UnlockAndPush(Vector2 dir, float impulse, float customUnlockTime = -1f)
    {
        if (customUnlockTime <= 0f) customUnlockTime = unlockTime;
        if (unlockCo != null) StopCoroutine(unlockCo);
        unlockCo = StartCoroutine(CoUnlockAndPush(dir.normalized, impulse, customUnlockTime));
    }

    IEnumerator CoUnlockAndPush(Vector2 dir, float impulse, float t)
    {
        Unlock();
        rb.AddForce(dir * impulse, ForceMode2D.Impulse);
        yield return new WaitForSeconds(t);
        if (zeroOutOnRelock)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
        Lock();
        unlockCo = null;
    }
}