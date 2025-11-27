using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("Referencias UI")]
    public GameObject dialogueBox;
    public TMP_Text dialogueText;

    [Header("Configuración")]
    public float typingSpeed = 0.03f;

    [Tooltip("Si está activo, el juego se pausa mientras haya un diálogo abierto.")]
    public bool pauseGameWhileDialogue = true;

    private Queue<string> sentences;
    private bool isShowingDialogue = false;
    private bool isTyping = false;
    private string currentSentence = "";

    private Action onDialogueFinished;
    private float previousTimeScale = 1f;

    // ⬇️ NUEVO: propiedad pública y estática para consultar desde fuera
    public bool IsDialogueActive => isShowingDialogue;
    public static bool IsAnyDialogueActive => Instance != null && Instance.isShowingDialogue;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        sentences = new Queue<string>();

        if (dialogueBox != null)
            dialogueBox.SetActive(false);
    }

    void Update()
    {
        if (!isShowingDialogue) return;

        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogueText.text = currentSentence;
                isTyping = false;
            }
            else
            {
                DisplayNextSentence();
            }
        }
    }

    public void StartDialogue(Dialogue dialogue, Action onFinished = null)
    {
        if (dialogue == null) return;

        // ⬇️ IMPORTANTE: si ya hay un diálogo, ignoramos la llamada
        if (isShowingDialogue)
            return;

        if (dialogueBox != null)
            dialogueBox.SetActive(true);

        isShowingDialogue = true;
        sentences.Clear();

        onDialogueFinished = onFinished;

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        if (pauseGameWhileDialogue)
        {
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
        }

        DisplayNextSentence();
    }

    void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        currentSentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentSentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
        isTyping = false;
    }

    void EndDialogue()
    {
        isShowingDialogue = false;

        if (dialogueBox != null)
            dialogueBox.SetActive(false);

        if (pauseGameWhileDialogue)
        {
            Time.timeScale = previousTimeScale;
        }

        var callback = onDialogueFinished;
        onDialogueFinished = null;
        callback?.Invoke();
    }
}