using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways] // <-- hace que corra también en el editor
public class HealthBarUI : MonoBehaviour
{
    public HealthSystem health;
    public UnityEngine.UI.Image fillImage;
    public Transform target;
    public Vector3 worldOffset = new Vector3(0f, 0.8f, 0f);
    public bool hideWhenFull = true;
    public float smooth = 15f;
    public float showOnHitForSeconds = 1.5f;
    public Gradient colorByPercent;

    CanvasGroup cg;
    float targetFill = 1f;
    Coroutine autoHideCR;

    void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        if (cg == null) cg = gameObject.AddComponent<CanvasGroup>();
        if (target == null && transform.parent != null) target = transform.parent;
        ForceRefresh();
        ApplyVisibilityInstant();
    }

    void LateUpdate()
    {
        // En editor o en Play, mantenla pegada al target
        if (target != null)
            transform.position = target.position + worldOffset;

        if (Application.isPlaying)
        {
            if (fillImage != null)
            {
                fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, targetFill, Time.deltaTime * smooth);
                if (colorByPercent != null)
                    fillImage.color = colorByPercent.Evaluate(fillImage.fillAmount);
            }
        }
        else
        {
            // En el editor, muéstrala siempre para poder colocarla
            if (cg != null) cg.alpha = 1f;
            ForceRefresh();
        }
    }

    public void RefreshFromEvent()
    {
        UpdateTargetFill();
        ShowTemporarily();
    }

    public void OnDeathHide()
    {
        targetFill = 0f;
        ApplyVisibility(0f);
    }

    void ForceRefresh()
    {
        UpdateTargetFill();
        if (fillImage != null) fillImage.fillAmount = targetFill;
        if (colorByPercent != null && fillImage != null)
            fillImage.color = colorByPercent.Evaluate(fillImage.fillAmount);
    }

    void UpdateTargetFill()
    {
        if (health == null) return;
        float denom = Mathf.Max(0.0001f, health.maxHealth);
        targetFill = Mathf.Clamp01(health.currentHealth / denom);
    }

    void ShowTemporarily()
    {
        ApplyVisibility(1f);
        if (!Application.isPlaying) return;
        if (autoHideCR != null) StopCoroutine(autoHideCR);
        if (hideWhenFull) autoHideCR = StartCoroutine(AutoHide());
    }

    System.Collections.IEnumerator AutoHide()
    {
        yield return new WaitForSeconds(showOnHitForSeconds);
        if (hideWhenFull && Mathf.Approximately(targetFill, 1f))
            ApplyVisibility(0f);
        autoHideCR = null;
    }

    void ApplyVisibilityInstant()
    {
        if (!hideWhenFull) { ApplyVisibility(1f); return; }
        ApplyVisibility(Mathf.Approximately(targetFill, 1f) ? 0f : 1f);
    }

    void ApplyVisibility(float a)
    {
        if (cg == null) return;
        cg.alpha = a;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }
}
