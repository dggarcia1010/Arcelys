using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // importante

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("Referencias UI")]
    public GameObject dialogueBox;       // aquí va el prefab instanciado o el objeto en escena
    public TMP_Text dialogueText;        // el TextMeshPro dentro del cuadro

    [Header("Configuración")]
    public float typingSpeed = 0.03f;    // velocidad de escritura

    private Queue<string> sentences;
    private bool isShowingDialogue = false;
    private bool isTyping = false;
    private string currentSentence = "";

    void Awake()
    {
        // Singleton sencillo
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        sentences = new Queue<string>();

        if (dialogueBox != null)
            dialogueBox.SetActive(false);  // asegurarse de que empieza oculto
    }

    void Update()
    {
        if (!isShowingDialogue) return;

        // Tecla para avanzar (puedes dejar sólo Space si quieres)
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                // Terminar de escribir la frase de golpe
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

    public void StartDialogue(Dialogue dialogue)
    {
        if (dialogueBox != null)
            dialogueBox.SetActive(true);

        isShowingDialogue = true;
        sentences.Clear();

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
    }
}