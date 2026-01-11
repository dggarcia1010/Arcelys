// TreeDebug.cs - Agrégale este script temporalmente al Tree
using UnityEngine;

public class TreeDebug : MonoBehaviour
{
    void Awake()
    {
        Debug.Log("Tree - Awake: " + gameObject.activeSelf);
    }
    
    void Start()
    {
        Debug.Log("Tree - Start: " + gameObject.activeSelf);
    }
    
    void OnEnable()
    {
        Debug.Log("Tree - OnEnable: " + gameObject.activeSelf);
    }
}
