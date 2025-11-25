using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class FlammableTorch : MonoBehaviour
{
    public Light2D flame;
    public TorchSimonPuzzle puzzle;

    private Coroutine autoOffRoutine;

    void Awake()
    {
        if (flame != null)
            flame.enabled = false;
    }

    // Llamado cuando el jugador enciende la antorcha (proyectil)
    public void TurnOn()
    {
        flame.enabled = true;

        // Reiniciar temporizador auto-apagado
        if (autoOffRoutine != null)
            StopCoroutine(autoOffRoutine);

        autoOffRoutine = StartCoroutine(AutoTurnOff());

        // Informar al puzzle
        puzzle?.TorchActivated(this);
    }

    // Apagado automático después de 2 segundos
    private IEnumerator AutoTurnOff()
    {
        yield return new WaitForSeconds(2f);
        TurnOff();
    }

    public void TurnOff()
    {
        if (flame != null)
            flame.enabled = false;
    }

    // Para mostrar la secuencia del puzzle
    public void ShowPuzzleFlash()
    {
        flame.enabled = true;

        // Asegurar refresco del render del Light2D
        flame.intensity = flame.intensity;

        // Apagado después del showTime viene desde el controller
    }
}