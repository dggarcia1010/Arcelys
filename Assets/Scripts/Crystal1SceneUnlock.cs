using System.Collections;
using UnityEngine;

public class Crystal1SceneUnlock : MonoBehaviour
{
    [Header("Condición")]
    public string enemyTag = "Enemy";

    [Header("Diálogo al completar")]
    public Dialogue completeDialogue;

    [Header("Reliquia (GameObject oculto con MagicSpriteEffect)")]
    public GameObject relicObject;

    [Header("Aparición sobre el jugador")]
    public Vector3 relicOffset = new Vector3(0f, 0.8f, 0f);

    [Header("Seguridad")]
    public bool oneTime = true;

    private bool triggered = false;
    private Transform playerTransform;
    private MonoBehaviour playerMovement; // <- script de movimiento

    void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;

            // 🔒 Buscamos el script de movimiento (NO el Rigidbody)
            playerMovement = player.GetComponent<PlayerMovement>();
        }
    }

    void Update()
    {
        if (triggered && oneTime) return;

        if (CrystalProgress.Instance != null && CrystalProgress.Instance.crystal1Unlocked)
        {
            triggered = true;
            return;
        }

        int enemies = GameObject.FindGameObjectsWithTag(enemyTag).Length;
        if (enemies > 0) return;

        triggered = true;
        StartCoroutine(RunSequence());
    }

    IEnumerator RunSequence()
    {
        // 1️⃣ Diálogo (aquí ya se pausa el juego por tu DialogueManager)
        if (DialogueManager.Instance != null && completeDialogue != null)
        {
            bool finished = false;
            DialogueManager.Instance.StartDialogue(completeDialogue, () => finished = true);
            while (!finished) yield return null;
        }

        // 2️⃣ Congelar movimiento del player
        if (playerMovement != null)
            playerMovement.enabled = false;

        // 3️⃣ Colocar la reliquia encima del player y lanzar animación
        if (relicObject != null && playerTransform != null)
        {
            relicObject.transform.position = playerTransform.position + relicOffset;

            if (!relicObject.activeSelf)
                relicObject.SetActive(true);

            relicObject.GetComponent<MagicSpriteEffect>()?.TriggerEffect();
        }

        // 4️⃣ Esperar a que termine la animación
        float totalEffectTime = 0f;
        if (relicObject != null)
        {
            MagicSpriteEffect effect = relicObject.GetComponent<MagicSpriteEffect>();
            if (effect != null)
            {
                totalEffectTime =
                    effect.fadeInDuration +
                    effect.riseDuration +
                    effect.glowDuration +
                    effect.fadeOutDuration;
            }
        }

        if (totalEffectTime > 0f)
            yield return new WaitForSeconds(totalEffectTime);

        // 5️⃣ Descongelar movimiento del player
        if (playerMovement != null)
            playerMovement.enabled = true;

        // 6️⃣ Guardar progreso
        CrystalProgress.Instance?.UnlockCrystal1();

        // 7️⃣ Fade del cristal en el HUD
        if (CrystalHUD.Instance != null)
        {
            CrystalHUD.Instance.RefreshAll();
            CrystalHUD.Instance.FadeInCrystal1();
        }
    }
}