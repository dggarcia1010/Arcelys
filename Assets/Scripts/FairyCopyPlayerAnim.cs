using UnityEngine;

public class FairyCopyPlayerAnim : MonoBehaviour
{
    [Header("Referencias (si no se asignan, se buscan solas)")]
    [SerializeField] private Animator fairyAnimator;
    [SerializeField] private Animator playerAnimator;

    [Header("Control")]
    [SerializeField] private bool copyEnabled = false;

    [Header("Dirección cuando NO copia")]
    [SerializeField] private Vector2 idleDirWhenDisabled = Vector2.down; // (0,-1)

    void Awake()
    {
        // Busca Animator del hada en este objeto o hijos
        if (!fairyAnimator)
            fairyAnimator = GetComponentInChildren<Animator>(true);

        // Busca Animator del player en el Player o en hijos
        if (!playerAnimator)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p)
            {
                playerAnimator = p.GetComponentInChildren<Animator>(true);
            }
        }

        // Cuando empieza desactivado: fuerza idle down para que no se quede raro
        if (!copyEnabled)
            ForceIdleDirection();
    }

    void LateUpdate()
    {
        if (!fairyAnimator || !playerAnimator) return;

        if (!copyEnabled)
            return;

        fairyAnimator.SetFloat("moveX", playerAnimator.GetFloat("moveX"));
        fairyAnimator.SetFloat("moveY", playerAnimator.GetFloat("moveY"));
        fairyAnimator.SetBool("moving", playerAnimator.GetBool("moving"));
    }

    public void SetCopyEnabled(bool enabled)
    {
        copyEnabled = enabled;

        if (!copyEnabled)
            ForceIdleDirection();
    }

    private void ForceIdleDirection()
    {
        if (!fairyAnimator) return;

        fairyAnimator.SetFloat("moveX", idleDirWhenDisabled.x);
        fairyAnimator.SetFloat("moveY", idleDirWhenDisabled.y);
        fairyAnimator.SetBool("moving", false);
    }
}