using UnityEngine;

public class FairySpellSkinSwitcher : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerSpells playerSpells;
    [SerializeField] private Animator fairyAnimator;

    [Header("Overrides")]
    public AnimatorOverrideController windOverride;
    public AnimatorOverrideController iceOverride;
    public AnimatorOverrideController fireOverride;

    private PlayerSpells.SpellType lastSpell = (PlayerSpells.SpellType)(-1);

    void Awake()
    {
        if (!fairyAnimator)
            fairyAnimator = GetComponentInChildren<Animator>(true);

        if (!playerSpells)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) playerSpells = p.GetComponent<PlayerSpells>();
        }

        Apply(playerSpells ? playerSpells.CurrentSpell : PlayerSpells.SpellType.Wind);
    }

    void Update()
    {
        if (!playerSpells || !fairyAnimator) return;

        var spell = playerSpells.CurrentSpell;
        if (spell == lastSpell) return;

        Apply(spell);
    }

    void Apply(PlayerSpells.SpellType spell)
    {
        lastSpell = spell;

        switch (spell)
        {
            case PlayerSpells.SpellType.Wind:
                if (windOverride)
                    fairyAnimator.runtimeAnimatorController = windOverride;
                break;

            case PlayerSpells.SpellType.Ice:
                if (iceOverride)
                    fairyAnimator.runtimeAnimatorController = iceOverride;
                break;

            case PlayerSpells.SpellType.Fire:
                if (fireOverride)
                    fairyAnimator.runtimeAnimatorController = fireOverride;
                break;
        }
    }
}