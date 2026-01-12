using System.Collections;
using UnityEngine;

public class Crystal2BossUnlock : MonoBehaviour
{
    [Header("Referencia al trigger del boss (NO se modifica)")]
    public EnemyGroupDialogueTrigger bossTrigger;

    [Header("Reliquia 2 (GameObject oculto con MagicSpriteEffect)")]
    public GameObject relicObject;

    [Header("Aparece encima del jugador")]
    public Vector3 relicOffset = new Vector3(0f, 0.8f, 0f);

    [Header("Congelar movimiento")]
    public bool freezePlayerDuringEffect = true;

    private bool done = false;
    private Transform playerTransform;
    private MonoBehaviour playerMovement;

    void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerMovement = player.GetComponent<PlayerMovement>(); // tu script de movimiento
        }
    }

    void Update()
    {
        if (done) return;

        // Si ya está desbloqueado globalmente, no repetir
        if (CrystalProgress.Instance != null && CrystalProgress.Instance.crystal2Unlocked)
        {
            done = true;
            return;
        }

        if (bossTrigger == null) return;

        // Esperamos a que el bossTrigger haya terminado su condición (todos muertos)
        // y que el diálogo ya haya pasado (cuando ya no hay diálogo activo)
        if (AreEnemiesDeadInTrigger() && !DialogueManager.IsAnyDialogueActive)
        {
            done = true;
            StartCoroutine(DoUnlockSequence());
        }
    }

    bool AreEnemiesDeadInTrigger()
    {
        if (bossTrigger.enemies == null || bossTrigger.enemies.Length == 0) return false;

        foreach (var e in bossTrigger.enemies)
        {
            if (e != null && e.currentHealth > 0)
                return false;
        }
        return true;
    }

    IEnumerator DoUnlockSequence()
    {
        // Justo por seguridad: si en este frame se abre diálogo, esperamos a que termine
        while (DialogueManager.IsAnyDialogueActive)
            yield return null;

        // Congelar player
        if (freezePlayerDuringEffect && playerMovement != null)
            playerMovement.enabled = false;

        // Colocar reliquia sobre el player y ejecutar efecto
        float totalEffectTime = 0f;

        if (relicObject != null && playerTransform != null)
        {
            relicObject.transform.position = playerTransform.position + relicOffset;

            if (!relicObject.activeSelf)
                relicObject.SetActive(true);

            // Si tu MagicSpriteEffect tiene playOnStart=false, esto lo dispara sí o sí
            var effect = relicObject.GetComponent<MagicSpriteEffect>();
            if (effect != null)
            {
                effect.TriggerEffect();
                totalEffectTime =
                    effect.fadeInDuration +
                    effect.riseDuration +
                    effect.glowDuration +
                    effect.fadeOutDuration;
            }
        }

        if (totalEffectTime > 0f)
            yield return new WaitForSeconds(totalEffectTime);

        // Descongelar player
        if (freezePlayerDuringEffect && playerMovement != null)
            playerMovement.enabled = true;

        // Guardar progreso cristal 2
        CrystalProgress.Instance?.UnlockCrystal2();

        // HUD: refrescar + fade del cristal 2
        if (CrystalHUD.Instance != null)
        {
            CrystalHUD.Instance.RefreshAll();
            CrystalHUD.Instance.FadeInCrystal2();
        }
    }
}