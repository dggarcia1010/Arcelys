using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class TorchSimonPuzzle : MonoBehaviour
{
    [Header("Antorchas")]
    public List<FlammableTorch> torches;

    [Header("Patrones por ronda")]
    public int[] patternLengths = { 3, 4, 5 };

    [Header("Configuración")]
    public float showTime = 1f;
    public float delayBetween = 0.4f;
    public float timeLimit = 10f;
    public float nextRoundDelay = 2f;

    [Header("UI")]
    public TMP_Text uiText;
    public TMP_Text uiTimerText;
    public TMP_Text uiRoundText;

    [Header("Final del puzzle")]
    public Collider2D colliderToDisable;
    public bool disableObject = true;
    public GameObject rewardToShow;
    public Dialogue puzzleCompleteDialogue;

    // Estado interno
    private List<int> sequence = new();
    private int inputIndex = 0;
    private float timer = 0f;

    private bool puzzleActive = false;
    private bool isShowingSequence = false;
    private bool playerInside = false;
    private bool waitingForRestart = false;

    private int round = 1;
    public int maxRounds = 4;

    void Start()
    {
        ClearUI();
    }

    void Update()
    {
        // ▶️ Empezar o repetir SOLO tras fallo
        if (playerInside && waitingForRestart && Input.GetKeyDown(KeyCode.E))
        {
            waitingForRestart = false;
            round = 1;
            StartCoroutine(StartRound());
        }

        // ▶️ Empezar primera vez
        if (playerInside && !puzzleActive && !isShowingSequence && !waitingForRestart && round == 1 && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(StartRound());
        }

        // ⏱️ Timer de la ronda
        if (puzzleActive)
        {
            timer -= Time.deltaTime;
            SetTimerText($"Tiempo: {Mathf.Max(0f, timer):0.0}s");

            if (timer <= 0f)
                PuzzleFailed();
        }
        else
        {
            SetTimerText("");
        }
    }

    // =======================
    // PUZZLE CORE
    // =======================

    IEnumerator StartRound()
    {
        puzzleActive = false;
        isShowingSequence = true;

        TurnOffAllTorches();
        yield return new WaitForSeconds(0.25f);

        sequence.Clear();

        int lengthIndex = Mathf.Clamp(round - 1, 0, patternLengths.Length - 1);
        int patternLength = patternLengths[lengthIndex];

        SetRoundText($"Ronda {round}/{maxRounds}");

        // ✅ Ajuste: quitamos el "Memoriza la secuencia..."
        SetMainText("");

        for (int i = 0; i < patternLength; i++)
            sequence.Add(Random.Range(0, torches.Count));

        yield return ShowSequence();

        // ▶️ Ya puede jugar
        inputIndex = 0;
        timer = timeLimit;
        puzzleActive = true;
        isShowingSequence = false;

        SetMainText("Repite la secuencia");
    }

    IEnumerator ShowSequence()
    {
        foreach (int index in sequence)
        {
            var t = torches[index];
            if (t == null) continue;

            t.ShowPuzzleFlash();
            yield return new WaitForSeconds(showTime);
            t.TurnOff();
            yield return new WaitForSeconds(delayBetween);
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
            SetMainText($"✔ Bien ({inputIndex}/{sequence.Count})");

            if (inputIndex >= sequence.Count)
                PuzzleCompletedRound();
        }
        else
        {
            PuzzleFailed();
        }
    }

    // =======================
    // RESULTADOS
    // =======================

    void PuzzleFailed()
    {
        puzzleActive = false;
        isShowingSequence = false;
        waitingForRestart = true;

        inputIndex = 0;
        round = 1;

        TurnOffAllTorches();
        SetTimerText("");
        SetRoundText("");

        // ✅ Ajuste: quitamos el emoji que te rompe la X
        SetMainText("Has fallado\nPulsa <b>E</b> para repetir");
    }

    void PuzzleCompletedRound()
    {
        puzzleActive = false;
        isShowingSequence = false;
        round++;

        if (round > maxRounds)
        {
            PuzzleCompleted();
            return;
        }

        SetMainText("Ronda superada");
        StartCoroutine(NextRoundCoroutine());
    }

    IEnumerator NextRoundCoroutine()
    {
        yield return new WaitForSeconds(nextRoundDelay);
        StartCoroutine(StartRound());
    }

    void PuzzleCompleted()
    {
        TurnOffAllTorches();
        SetTimerText("");
        SetRoundText($"Ronda {maxRounds}/{maxRounds}");
        SetMainText("🎉 ¡Puzzle completado!");

        if (puzzleCompleteDialogue != null && DialogueManager.Instance != null)
            DialogueManager.Instance.StartDialogue(puzzleCompleteDialogue, AfterDialogue);
        else
            AfterDialogue();
    }

    void AfterDialogue()
    {
        if (colliderToDisable != null)
        {
            if (disableObject)
                colliderToDisable.gameObject.SetActive(false);
            else
                colliderToDisable.isTrigger = true;
        }

        if (rewardToShow != null)
            rewardToShow.SetActive(true);
    }

    // =======================
    // HELPERS
    // =======================

    void TurnOffAllTorches()
    {
        foreach (var t in torches)
            if (t != null)
                t.TurnOff();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = true;

        if (!puzzleActive && !isShowingSequence && round == 1)
            SetMainText("Pulsa <b>E</b> para empezar");
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;
        ClearUI();

        puzzleActive = false;
        isShowingSequence = false;
        waitingForRestart = false;
        inputIndex = 0;
        round = 1;

        TurnOffAllTorches();
    }

    void ClearUI()
    {
        SetMainText("");
        SetTimerText("");
        SetRoundText("");
    }

    void SetMainText(string s)  { if (uiText != null) uiText.text = s; }
    void SetTimerText(string s) { if (uiTimerText != null) uiTimerText.text = s; }
    void SetRoundText(string s) { if (uiRoundText != null) uiRoundText.text = s; }
}