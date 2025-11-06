using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartOnDeath : MonoBehaviour
{
    [SerializeField] float delay = 1.0f; // para ver la animación/FX

    // Llama a esto desde el UnityEvent "On Death ()"
    public void ReloadScene()
    {
        // Si quieres retraso, usa Invoke; si no, llama directo a LoadScene
        Invoke(nameof(DoReload), delay);
    }

    void DoReload()
    {
        if (Time.timeScale == 0) Time.timeScale = 1f; // por si pausaste al morir
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
