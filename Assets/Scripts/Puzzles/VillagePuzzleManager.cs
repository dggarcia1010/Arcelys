using UnityEngine;

public class VillagePuzzleManager : MonoBehaviour
{
    public static VillagePuzzleManager Instance { get; private set; }

    [Header("Solo debug")]
    public bool puzzleCompleted = false;
    public NPCDialoguePuzzle lastTalkedNPC;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void TalkTo(NPCDialoguePuzzle npc)
    {
        if (npc == null) return;

        lastTalkedNPC = npc;

        if (puzzleCompleted) return;

        if (npc.isCorrectNPC)
        {
            puzzleCompleted = true;
            Debug.Log($"PUZZLE COMPLETADO: has hablado con el correcto ({npc.npcName})");

            // aquí luego:
            // - cambiar de escena
            // - abrir una puerta
            // - marcar misión como completada, etc.
        }
        else
        {
            Debug.Log($"Este no es el aldeano correcto: {npc.npcName}");
            // Las pistas las das en el propio Dialogue del NPC.
        }
    }
}
