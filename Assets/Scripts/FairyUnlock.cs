using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider2D))]
public class FairyUnlock : MonoBehaviour
{
    [Header("Asignaciones (opcionales, se autocompletan)")]
    public FollowFairy followFairy;
    public PlayerSpells playerSpells;
    public string playerTag = "Player";
    public KeyCode interactKey = KeyCode.E;

    [Header("UI Mensaje")]
    public GameObject interactText; // tooltip encima del hada

    [Header("Panel de instrucciones")]
    public InstructionPanel instructionPanel; // <-- arrástralo desde el Canvas

    [Header("Estado")]
    public bool unlocked = false;
    public bool oneTime = true;

    Collider2D triggerCol;

    void Awake()
    {
        triggerCol = GetComponent<Collider2D>();
        triggerCol.isTrigger = true;

        if (followFairy == null) followFairy = GetComponentInParent<FollowFairy>();

        if (playerSpells == null)
        {
            var p = GameObject.FindGameObjectWithTag(playerTag);
            if (p) playerSpells = p.GetComponent<PlayerSpells>();
        }

        if (interactText != null)
            interactText.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!unlocked && other.CompareTag(playerTag) && interactText != null)
            interactText.SetActive(true);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (unlocked) return;
        if (!other.CompareTag(playerTag)) return;

        if (Input.GetKeyDown(interactKey))
        {
            Unlock();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) && interactText != null)
            interactText.SetActive(false);
    }

    void Unlock()
    {
        unlocked = true;

        if (playerSpells != null) playerSpells.UnlockMagic();

        if (followFairy != null)
        {
            followFairy.SetFairyActive(true);
            var phys = followFairy.GetComponent<CircleCollider2D>();
            if (phys != null) phys.isTrigger = true;
        }

        if (interactText != null)
            interactText.SetActive(false);

        // 👇 Mostrar panel con instrucciones
        if (instructionPanel != null)
        {
            instructionPanel.Show(
                "Pulsa 1 (Viento), 2 (Hielo), 3 (Fuego), 4 (Luz) para seleccionar el hechizo.\nPulsa ESPACIO para lanzarlo hacia el ratón.",
                0f // 0 = no autocierra; si quieres auto-ocultar en 4s, cambia a 4f
            );
        }

        if (oneTime) Destroy(this);
        Debug.Log("Magia desbloqueada y hada activada.");
    }
}