using System.Collections;
using UnityEngine;

public class CatMeow : MonoBehaviour
{
    public AudioClip meow;
    public float minTime = 5f;
    public float maxTime = 12f;

    private AudioSource a;

    void Start()
    {
        a = GetComponent<AudioSource>();
        if (a == null) a = gameObject.AddComponent<AudioSource>();

        a.playOnAwake = false;
        a.loop = false;
        a.spatialBlend = 1f; // 3D para que se oiga según distancia

        StartCoroutine(MeowRoutine());
    }

    IEnumerator MeowRoutine()
    {
        while (true)
        {
            float wait = Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(wait);

            a.PlayOneShot(meow);
        }
    }
}
