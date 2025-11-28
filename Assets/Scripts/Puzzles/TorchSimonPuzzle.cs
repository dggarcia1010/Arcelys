using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TorchSimonPuzzle : MonoBehaviour
{
    [Header("Antorchas en orden (1–6)")]
    public List<FlammableTorch> torches;

    [Header("Patrones por ronda")]
    [Tooltip("Longitudes de la secuencia por ronda. Ronda 1 usa el índice 0.")]
    public int[] patternLengths = { 3, 4, 5, 6, 7 };

    [Header("Configuración del puzzle")]
    public float showTime = 1f;
    public float delayBetween = 0.4f;
    public float timeLimit = 10f;
    public float nextRoundDelay = 2f;

    [Header("Acciones al completar puzzle")]
    [Tooltip("Collider que desaparecerá al completar la última ronda del puzzle.")]
    public Collider2D colliderToDisable;
    [Tooltip("Si está activado, se desactiva todo el GameObject del collider. Si no, solo se pone en isTrigger.")]
    public bool disableObject = true;

    [Header("Diálogo opcional al completar puzzle")]
    public Dialogue puzzleCompleteDialogue;

    private List<int> sequence = new List<int>();
    private int inputIndex = 0;
    private float timer = 0f;
    private bool puzzleActive = false;
    private bool playerInside = false;
    private bool isShowingSequence = false;

    private int round = 1;
    public int maxRounds = 5;

    void Update()
    {
        if (playerInside && !puzzleActive && !isShowingSequence && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Puzzle iniciado");
            StartCoroutine(StartRound());
        }

        if (puzzleActive)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
                PuzzleFailed();
        }
    }

    public void TorchActivated(FlammableTorch torch)
    {
        if (!puzzleActive) return;

        int idx = torches.IndexOf(torch);
        if (idx == -1) return;

        if (idx == sequence[inputIndex])
        {
            inputIndex++;

            if (inputIndex >= sequence.Count)
            {
                PuzzleCompletedRound();
            }
        }
        else
        {
            PuzzleFailed();
        }
    }

    IEnumerator StartRound()
    {
        puzzleActive = false;
        isShowingSequence = true;

        TurnOffAllTorches();
        yield return new WaitForSeconds(0.2f);

        sequence.Clear();

        int lengthIndex = Mathf.Clamp(round - 1, 0, patternLengths.Length - 1);
        int patternLength = patternLengths[lengthIndex];

        Debug.Log($"Ronda {round} → patrón de {patternLength} pasos");

        for (int i = 0; i < patternLength; i++)
            sequence.Add(Random.Range(0, torches.Count));

        yield return ShowSequence();

        inputIndex = 0;
        timer = timeLimit;
        puzzleActive = true;
        isShowingSequence = false;

        Debug.Log("Introduce la secuencia ahora.");
    }

    IEnumerator ShowSequence()
    {
        Debug.Log("Mostrando secuencia…");

        foreach (int index in sequence)
        {
            var t = torches[index];
            if (t == null) continue;

            t.ShowPuzzleFlash();
            yield return null;
            yield return new WaitForSeconds(showTime);
            t.TurnOff();
            yield return new WaitForSeconds(delayBetween);
        }
    }

    void PuzzleFailed()
    {
        Debug.Log("❌ Puzzle fallado. Reinicia con E.");

        puzzleActive = false;
        inputIndex = 0;
        isShowingSequence = false;
        round = 1;

        TurnOffAllTorches();
    }

    void PuzzleCompletedRound()
    {
        Debug.Log("✔ Ronda superada");

        puzzleActive = false;
        isShowingSequence = false;

        round++;

        if (round > maxRounds)
        {
            Debug.Log("🎉 Puzzle COMPLETADO");
            TurnOffAllTorches();

            // 🔥 Lanzar diálogo si existe
            if (puzzleCompleteDialogue != null && DialogueManager.Instance != null)
            {
                DialogueManager.Instance.StartDialogue(puzzleCompleteDialogue, AfterDialogue);
            }
            else
            {
                AfterDialogue();
            }

            return;
        }

        StartCoroutine(NextRoundCoroutine());
    }

    IEnumerator NextRoundCoroutine()
    {
        yield return new WaitForSeconds(0.25f);
        TurnOffAllTorches();
        yield return new WaitForSeconds(nextRoundDelay);
        StartCoroutine(StartRound());
    }

    void AfterDialogue()
    {
        if (colliderToDisable != null)
        {
            if (disableObject)
            {
                colliderToDisable.gameObject.SetActive(false);
                Debug.Log("✔ Collider desactivado: " + colliderToDisable.name);
            }
            else
            {
                colliderToDisable.isTrigger = true;
                Debug.Log("✔ Collider puesto como trigger: " + colliderToDisable.name);
            }
        }
    }

    void TurnOffAllTorches()
    {
        foreach (var t in torches)
            if (t != null)
                t.TurnOff();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            Debug.Log("Estás en el puzzle. Pulsa E para comenzar.");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }
}