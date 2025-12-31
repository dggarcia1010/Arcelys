using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("UI")]
    public GameObject pausePanel;
    public GameObject mainMenu;
    public GameObject settingsMenu;

    [Header("Brightness")]
    public Image brightnessOverlay;
    public Slider brightnessSlider;

    bool isPaused = false;
    const string BRIGHTNESS_KEY = "brightness";

    // Para que nunca se quede negro total
    const float MIN_BRIGHTNESS = 0.2f; // ajusta si quieres (0.3/0.4...)

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        float savedBrightness = PlayerPrefs.GetFloat(BRIGHTNESS_KEY, 1f);
        savedBrightness = Mathf.Clamp(savedBrightness, MIN_BRIGHTNESS, 1f);
        ApplyBrightness(savedBrightness);
    }

    void Start()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (mainMenu != null) mainMenu.SetActive(true);
        if (settingsMenu != null) settingsMenu.SetActive(false);

        if (brightnessSlider != null)
        {
            float b = PlayerPrefs.GetFloat(BRIGHTNESS_KEY, 1f);
            b = Mathf.Clamp(b, MIN_BRIGHTNESS, 1f);
            brightnessSlider.SetValueWithoutNotify(b);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        if (pausePanel != null) pausePanel.SetActive(true);
        ShowMainMenu();
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    public void RestartScene()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OpenSettings()
    {
        if (mainMenu != null) mainMenu.SetActive(false);
        if (settingsMenu != null) settingsMenu.SetActive(true);
    }

    public void BackFromSettings()
    {
        ShowMainMenu();
    }

    void ShowMainMenu()
    {
        if (mainMenu != null) mainMenu.SetActive(true);
        if (settingsMenu != null) settingsMenu.SetActive(false);
    }

    public void OnBrightnessChanged(float value)
    {
        value = Mathf.Clamp(value, MIN_BRIGHTNESS, 1f);
        ApplyBrightness(value);
        PlayerPrefs.SetFloat(BRIGHTNESS_KEY, value);
        PlayerPrefs.Save();
    }

    void ApplyBrightness(float value)
    {
        if (brightnessOverlay == null) return;

        Color c = brightnessOverlay.color;
        c.r = 0f; c.g = 0f; c.b = 0f;

        c.a = 1f - value;

        brightnessOverlay.color = c;
    }
}
