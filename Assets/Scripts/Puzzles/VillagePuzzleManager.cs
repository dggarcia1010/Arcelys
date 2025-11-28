using System.Collections.Generic;
using UnityEngine;

public class VillagePuzzleManager : MonoBehaviour
{
    public static VillagePuzzleManager Instance { get; private set; }

    [Header("Config")]
    [Tooltip("Número de aldeanos que tienes que haber hablado ANTES de poder hablar con el final.")]
    public int requiredPreviousNPCs = 5;

    [Header("Solo debug")]
    public bool puzzleCompleted = false;
    public NPCDialoguePuzzle lastTalkedNPC;
    public int talkedCountDebug;

    // Aldeanos ya hablados (los que NO son el final)
    private HashSet<NPCDialoguePuzzle> talkedSet = new HashSet<NPCDialoguePuzzle>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public bool HasMetRequirement => talkedSet.Count >= requiredPreviousNPCs;

    /// <summary>
    /// Devuelve si se permite hablar con este NPC.
    /// El NPC final (isCorrectNPC = true) solo es accesible cuando se ha hablado
    /// con requiredPreviousNPCs aldeanos previos.
    /// </summary>
    public bool CanTalkToNPC(NPCDialoguePuzzle npc)
    {
        if (npc == null) return false;

        // El NPC final solo se puede hablar si ya se han cumplido los requisitos
        if (npc.isCorrectNPC && !HasMetRequirement)
            return false;

        // Cualquier otro siempre se puede hablar
        return true;
    }

    /// <summary>
    /// Llamado por el NPC cuando el jugador inicia conversación con él.
    /// </summary>
    public void TalkTo(NPCDialoguePuzzle npc)
    {
        if (npc == null) return;

        lastTalkedNPC = npc;

        // Si NO es el NPC final, lo contamos como aldeano visitado
        if (!npc.isCorrectNPC)
        {
            if (talkedSet.Add(npc))
            {
                talkedCountDebug = talkedSet.Count;
                Debug.Log($"Has hablado con {talkedSet.Count}/{requiredPreviousNPCs} aldeanos necesarios.");
            }
        }

        if (puzzleCompleted) return;

        if (npc.isCorrectNPC)
        {
            // Si aún no se han hablado con los previos, NO completamos el puzzle
            if (!HasMetRequirement)
            {
                Debug.Log("Todavía no has hablado con todos los aldeanos necesarios.");
                return;
            }

            puzzleCompleted = true;
            Debug.Log($"PUZZLE COMPLETADO: has hablado con el correcto ({npc.npcName})");

            // Aquí luego:
            // - cambiar de escena
            // - abrir una puerta
            // - marcar misión como completada, etc.
        }
        else
        {
            Debug.Log($"Este no es el aldeano correcto: {npc.npcName}");
        }
    }
}