using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartOnDeath : MonoBehaviour
{
    [SerializeField] float delay = 1.0f;

    public void ReloadScene()
    {
        Invoke(nameof(DoReload), delay);
    }

    void DoReload()
    {
        if (Time.timeScale == 0) Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
