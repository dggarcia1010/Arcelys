using UnityEngine;
using System.Collections;
using System;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance { get; private set; }

    [Header("Referencias")]
    public CameraMovement cameraMovement;

    [Header("Config")]
    public bool pauseGame = true;

    private float prevTimeScale = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayCutscene(Transform focusPoint, float durationSeconds, Action onFinished = null)
    {
        StartCoroutine(PlayCutsceneRoutine(focusPoint, durationSeconds, onFinished));
    }

    private IEnumerator PlayCutsceneRoutine(Transform focus, float duration, Action callback)
    {
        if (cameraMovement == null) yield break;

        if (pauseGame)
        {
            prevTimeScale = Time.timeScale;
            Time.timeScale = 0f;
        }

        cameraMovement.StartCutscene(focus);

        yield return new WaitForSecondsRealtime(duration);

        cameraMovement.EndCutscene();

        if (pauseGame)
            Time.timeScale = prevTimeScale;

        callback?.Invoke();
    }
}
