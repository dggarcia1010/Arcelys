using UnityEngine;

public class CatQuestManager : MonoBehaviour
{
    public static CatQuestManager Instance { get; private set; }

    [Header("Estado de la misión del gato")]
    public bool missionAccepted = false;   
    public bool catRescued = false;        

    [Header("Opcional: collider a eliminar al completar misión")]
    public string colliderToRemoveName = "ColliderPaso";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); 
    }

    public void AcceptMission()
    {
        missionAccepted = true;
        Debug.Log("Misión del gato aceptada.");
    }

    public void CompleteMission()
    {
        catRescued = true;
        Debug.Log("Misión del gato completada.");

        TryRemoveColliderPaso();
    }


    void TryRemoveColliderPaso()
    {
        GameObject blocker = GameObject.Find(colliderToRemoveName);

        if (blocker != null)
        {
            Debug.Log("ColliderPaso encontrado y eliminado.");
            Destroy(blocker);
        }
        else
        {
            Debug.LogWarning("ColliderPaso no encontrado en esta escena.");
        }
    }
}