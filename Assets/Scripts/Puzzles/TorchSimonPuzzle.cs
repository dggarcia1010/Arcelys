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
        // Empezar puzzle con E
        if (playerInside && !puzzleActive && !isShowingSequence && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Puzzle iniciado");
            StartCoroutine(StartRound());
        }

        // Cuenta atrás mientras el jugador responde
        if (puzzleActive)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
                PuzzleFailed();
        }
    }

    // Llamado por FlammableTorch cuando el jugador la enciende con fuego
    public void TorchActivated(FlammableTorch torch)
    {
        if (!puzzleActive) return;

        int idx = torches.IndexOf(torch);
        if (idx == -1) return;

        if (idx == sequence[inputIndex])
        {
            inputIndex++;

            // Ha acertado TODA la secuencia
            if (inputIndex >= sequence.Count)
            {
                PuzzleCompletedRound();
            }
        }
        else
        {
            // Fallo → reiniciar
            PuzzleFailed();
        }
    }

    IEnumerator StartRound()
    {
        puzzleActive = false;
        isShowingSequence = true;

        // APAGAR TODAS las antorchas antes de empezar
        TurnOffAllTorches();
        yield return new WaitForSeconds(0.2f);

        sequence.Clear();

        // Obtener longitud desde patrón
        int lengthIndex = Mathf.Clamp(round - 1, 0, patternLengths.Length - 1);
        int patternLength = patternLengths[lengthIndex];

        Debug.Log($"Ronda {round} → patrón de {patternLength} pasos");

        // Generar secuencia
        for (int i = 0; i < patternLength; i++)
            sequence.Add(Random.Range(0, torches.Count));

        // Mostrar la secuencia animada
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

        List<int> seq = new List<int>(sequence);

        foreach (int index in seq)
        {
            if (index < 0 || index >= torches.Count)
                continue;

            var t = torches[index];
            if (t == null) continue;

            // Encender antorcha
            t.ShowPuzzleFlash();

            // Dejar un frame para refrescar el Light2D
            yield return null;

            yield return new WaitForSeconds(showTime);

            // Apagarla
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
            return;
        }

        StartCoroutine(NextRoundCoroutine());
    }

    IEnumerator NextRoundCoroutine()
    {
        // Esperar 0.25s para que SE VEA la última antorcha encendida
        yield return new WaitForSeconds(0.25f);

        // Apagar todo
        TurnOffAllTorches();

        // Pausa antes de demostrar nuevo patrón
        yield return new WaitForSeconds(nextRoundDelay);

        StartCoroutine(StartRound());
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