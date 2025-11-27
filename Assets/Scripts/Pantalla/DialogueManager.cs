using System;                      // ⬅️ NUEVO
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // si usas TextMeshPro

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("Referencias UI")]
    public GameObject dialogueBox;       // el panel
    public TMP_Text dialogueText;        // el texto dentro

    [Header("Configuración")]
    public float typingSpeed = 0.03f;    // velocidad de "escritura"

    private Queue<string> sentences;
    private bool isShowingDialogue = false;
    private bool isTyping = false;
    private string currentSentence = "";

    // ⬇️ NUEVO: callback cuando termina el diálogo
    private Action onDialogueFinished;

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

        // tecla para avanzar diálogo (puedes cambiarla)
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                // terminar la frase instantáneamente
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

    // ⬇️ CAMBIA esto: ahora acepta un callback opcional
    public void StartDialogue(Dialogue dialogue, Action onFinished = null)
    {
        if (dialogue == null) return;

        if (dialogueBox != null)
            dialogueBox.SetActive(true);

        isShowingDialogue = true;
        sentences.Clear();

        onDialogueFinished = onFinished; // guardamos el callback

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
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
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    void EndDialogue()
    {
        isShowingDialogue = false;

        if (dialogueBox != null)
            dialogueBox.SetActive(false);

        // ⬇️ LLAMAMOS AL CALLBACK SI EXISTE
        var callback = onDialogueFinished;
        onDialogueFinished = null;
        callback?.Invoke();
    }
}