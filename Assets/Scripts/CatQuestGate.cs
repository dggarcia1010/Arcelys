using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CatQuestGate : MonoBehaviour
{
    [Header("Condición para desactivar este collider")]
    public bool requireCatRescued = true;   // ahora solo usamos catRescued

    void Start()
    {
        var quest = CatQuestManager.Instance;
        if (quest == null) return;

        // Si la misión del gato ya está completada, desactivamos este objeto
        if (requireCatRescued && quest.catRescued)
        {
            Debug.Log("CatQuestGate: misión del gato completada, desactivando ColliderPaso.");
            gameObject.SetActive(false);   // o Destroy(gameObject); si prefieres
        }
    }
}