using UnityEngine;

public class SceneIntroDialogue : MonoBehaviour
{
    public Dialogue introDialogue;

    void Start()
    {
        if (DialogueManager.Instance != null && introDialogue != null)
        {
            DialogueManager.Instance.StartDialogue(introDialogue);
        }
    }
}