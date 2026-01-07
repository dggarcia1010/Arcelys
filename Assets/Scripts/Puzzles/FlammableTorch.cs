using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class FlammableTorch : MonoBehaviour
{
    [Header("Visual (no afecta colisiones)")]
    [Tooltip("Objeto hijo con SpriteRenderer+Animator y/o Light2D.")]
    public GameObject visualRoot;

    [Tooltip("Si la luz no está dentro de visualRoot, asígnala aquí.")]
    public Light2D flame;

    public TorchSimonPuzzle puzzle;

    private Coroutine autoOffRoutine;

    void Awake()
    {
        SetVisual(false);
    }

    // Llamado cuando el jugador enciende la antorcha (proyectil)
    public void TurnOn()
    {
        SetVisual(true);

        // Reiniciar temporizador auto-apagado
        if (autoOffRoutine != null)
            StopCoroutine(autoOffRoutine);

        autoOffRoutine = StartCoroutine(AutoTurnOff());

        // Informar al puzzle
        puzzle?.TorchActivated(this);
    }

    private IEnumerator AutoTurnOff()
    {
        yield return new WaitForSeconds(2f);
        TurnOff();
    }

    public void TurnOff()
    {
        SetVisual(false);
    }

    // Para mostrar la secuencia del puzzle
    public void ShowPuzzleFlash()
    {
        SetVisual(true);
        // el apagado viene desde el controller (ShowSequence -> TurnOff)
    }

    private void SetVisual(bool on)
    {
        // 1) Enciende/apaga el hijo visual (sprites/anim)
        if (visualRoot != null)
            visualRoot.SetActive(on);

        // 2) Enciende/apaga la luz (por si está fuera del visualRoot)
        if (flame != null)
            flame.enabled = on;
    }
}