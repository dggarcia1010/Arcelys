using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Nombre de la primera escena jugable")]
    public string firstGameplaySceneName = "Escena1";

    public void PlayGame()
    {
        SceneManager.LoadScene(firstGameplaySceneName);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}