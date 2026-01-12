using UnityEngine;

public class DontDestroyOnLoadSimple : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}