using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EndGameAfterEnemiesDialogue : MonoBehaviour
{
    [Header("Condición")]
    public string enemyTag = "Enemy";

    [Header("Diálogo final")]
    public Dialogue finalDialogue;

    [Header("Luz global (URP 2D)")]
    public Light2D globalLight;                 // Arrastra aquí tu Global Light 2D
    public Color normalLightColor = Color.white;
    public float normalLightIntensity = 1f;

    [Header("Opciones")]
    public float delayAfterAllDead = 0.5f;
    public bool onlyOnce = true;

    private bool triggered = false;

    void Update()
    {
        if (triggered && onlyOnce) return;

        if (GameObject.FindGameObjectsWithTag(enemyTag).Length > 0)
            return;

        triggered = true;
        StartCoroutine(RunEndSequence());
    }

    IEnumerator RunEndSequence()
    {
        // 1️⃣ Ajustar luz global al matar al último enemigo
        RestoreGlobalLight();

        // 2️⃣ Pequeño delay opcional
        if (delayAfterAllDead > 0f)
            yield return new WaitForSeconds(delayAfterAllDead);

        // 3️⃣ Esperar a que termine cualquier diálogo previo
        while (DialogueManager.IsAnyDialogueActive)
            yield return null;

        // 4️⃣ Mostrar diálogo final
        if (DialogueManager.Instance != null && finalDialogue != null)
        {
            bool finished = false;
            DialogueManager.Instance.StartDialogue(finalDialogue, () => finished = true);
            while (!finished) yield return null;
        }

        // 5️⃣ Finalizar juego
        QuitGame();
    }

    void RestoreGlobalLight()
    {
        if (globalLight == null) return;

        globalLight.color = normalLightColor;
        globalLight.intensity = normalLightIntensity;
    }

    void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}