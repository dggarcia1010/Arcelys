using UnityEngine;

public class PauseBootstrapper : MonoBehaviour
{
    [SerializeField] private GameObject pauseManagerPrefab;

    void Awake()
    {
        if (PauseManager.Instance == null)
        {
            Instantiate(pauseManagerPrefab);
        }
        Destroy(gameObject);
    }
}
