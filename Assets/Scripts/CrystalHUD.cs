using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CrystalHUD : MonoBehaviour
{
    public static CrystalHUD Instance;

    [Header("Referencias UI")]
    public Image crystal1;
    public Image crystal2;
    public Image crystal3;

    [Header("Colores")]
    public Color unlockedColor = Color.white;
    public Color lockedColor = Color.black;

    [Header("Fade")]
    public float fadeDuration = 0.8f;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        RefreshAll();
    }

    public void RefreshAll()
    {
        var p = CrystalProgress.Instance;
        if (p == null) return;

        SetCrystalState(crystal1, p.crystal1Unlocked);
        SetCrystalState(crystal2, p.crystal2Unlocked);
        SetCrystalState(crystal3, p.crystal3Unlocked);
    }

    void SetCrystalState(Image img, bool unlocked)
    {
        if (!img) return;
        img.color = unlocked ? unlockedColor : lockedColor;
    }

    public void FadeInCrystal1()
    {
        if (!crystal1) return;
        StopAllCoroutines();
        StartCoroutine(FadeIn(crystal1));
    }

    public void FadeInCrystal2()
    {
        if (!crystal2) return;
        StopAllCoroutines();
        StartCoroutine(FadeIn(crystal2));
    }

    public void FadeInCrystal3()
    {
        if (!crystal3) return;
        StopAllCoroutines();
        StartCoroutine(FadeIn(crystal3));
    }

    IEnumerator FadeIn(Image img)
    {
        // Arranca “encendido” pero con alpha 0
        Color c = unlockedColor;
        c.a = 0f;
        img.color = c;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Clamp01(t / fadeDuration);
            c.a = a;
            img.color = c;
            yield return null;
        }

        c.a = 1f;
        img.color = c;
    }
}