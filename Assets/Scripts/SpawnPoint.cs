using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [Tooltip("ID de este punto (ej: CampoSalidaCasa, CasaPuertaEntrada...)")]
    public string spawnId;

    void Start()
    {
        // ¿Es este el spawn que pidió el portal?
        if (!string.IsNullOrEmpty(ScenePortalFade.nextSpawnId) &&
            ScenePortalFade.nextSpawnId == spawnId)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = transform.position;
            }
        }
    }
}