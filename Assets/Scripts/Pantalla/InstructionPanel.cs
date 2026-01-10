using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InstructionPanel : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject panelRoot;            
    public TextMeshProUGUI messageText;    
    public Button closeButton;              

    [Header("Comportamiento")]
    public bool closeOnAnyKey = true;      
    public float autoHideSeconds = 0f;      
    public bool pauseGameWhenOpen = true;   

    private bool isOpen = false;
    private float hideTimer = 0f;

    void Awake()
    {
        if (panelRoot == null)
            panelRoot = gameObject;

        SetOpen(false);

        if (closeButton != null)
            closeButton.onClick.AddListener(() => SetOpen(false));
    }

    void Update()
    {
        if (!isOpen) return;

        if (closeOnAnyKey && Input.anyKeyDown)
        {
            SetOpen(false);
            return;
        }

        if (autoHideSeconds > 0f)
        {
            hideTimer -= Time.unscaledDeltaTime;
            if (hideTimer <= 0f)
            {
                SetOpen(false);
            }
        }
    }

    public void Show(string text = null, float duration = 0f)
    {
        if (messageText != null && !string.IsNullOrEmpty(text))
            messageText.text = text;

        autoHideSeconds = duration;
        hideTimer = duration;
        SetOpen(true);
    }

    public void SetOpen(bool open)
    {
        isOpen = open;
        if (panelRoot != null)
            panelRoot.SetActive(open);

        if (pauseGameWhenOpen)
        {
            Time.timeScale = open ? 0f : 1f;
        }
    }
}