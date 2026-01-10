using System.Collections;
using UnityEngine;

public class EnemyGroupDialogueTrigger : MonoBehaviour
{
    [Header("Enemigos que deben morir")]
    public EnemyHealth[] enemies;          // arrastra aquí los EnemyHealth de tus enemigos

    [Header("Diálogo a mostrar después")]
    public Dialogue dialogue;              // diálogo que quieres mostrar

    [Header("Configuración")]
    public float delayAfterAllDead = 1f;   // segundos a esperar tras morir el último

    [Header("Acción extra al terminar el diálogo")]
    public FairyMover fairyToMove;         // ⬅️ hada que se moverá tras el diálogo (opcional)

    private bool triggered = false;

    void Update()
    {
        if (triggered) return;
        if (AllEnemiesDead())
        {
            triggered = true;
            StartCoroutine(ShowDialogueAfterDelay());
        }
    }

    bool AllEnemiesDead()
    {
        if (enemies == null || enemies.Length == 0) return false;

        foreach (var e in enemies)
        {
            if (e != null && e.currentHealth > 0)
                return false;
        }

        return true;
    }

    IEnumerator ShowDialogueAfterDelay()
    {
        yield return new WaitForSeconds(delayAfterAllDead);

        if (DialogueManager.Instance != null && dialogue != null)
        {
            // Si hay hada asignada, la movemos al acabar el diálogo
            if (fairyToMove != null)
            {
                DialogueManager.Instance.StartDialogue(dialogue, () =>
                {
                    fairyToMove.StartMove();
                });
            }
            else
            {
                // Versión simple: solo diálogo
                DialogueManager.Instance.StartDialogue(dialogue);
            }
        }
    }
}