using UnityEngine;

// TAUNT SKILL

[CreateAssetMenu(menuName = "ADDON/Skills/Target/Taunt", order = 999)]
public class Skill_Taunt : ScriptableSkill
{
    [Header("[-=-[ Taunt Skill ]-=-]")]
    public LinearFloat successChance;

    public string tauntMessage = "You taunted: ";
    public string failedMessage = "You failed to taunt: ";

    // -----------------------------------------------------------------------------------
    // CheckTarget
    // -----------------------------------------------------------------------------------
    public override bool CheckTarget(Entity caster)
    {
        return caster.target != null && caster.CanAttack(caster.target);
    }

    // -----------------------------------------------------------------------------------
    // CheckDistance
    // -----------------------------------------------------------------------------------
#if _iMMO2D
    public override bool CheckDistance(Entity caster, int skillLevel, out Vector2 destination)
#else
    public override bool CheckDistance(Entity caster, int skillLevel, out Vector3 destination)
#endif
    {
        if (caster.target != null)
        {
            destination = caster.target.collider.ClosestPointOnBounds(caster.transform.position);
            return Utils.ClosestDistance(caster, caster.target) <= castRange.Get(skillLevel);
        }
        destination = caster.transform.position;
        return false;
    }

    // -----------------------------------------------------------------------------------
    // Apply
    // -----------------------------------------------------------------------------------
#if _iMMO2D
    public override void Apply(Entity caster, int skillLevel, Vector2 target)
#else
    public override void Apply(Entity caster, int skillLevel)
#endif
    {
#if _SERVER
        Player player = (Player)caster;

        if (player.target != null && player.target is Monster)
        {
            Monster monster = player.target.GetComponent<Monster>();

            if (UnityEngine.Random.value <= successChance.Get(skillLevel))
            {
                monster.Tools_OnAggro(player, 1);
                player.Tools_TargetAddMessage(tauntMessage + monster.name);
            }
            else
            {
                player.Tools_TargetAddMessage(failedMessage + monster.name);
            }
        }
#endif
    }
    // -----------------------------------------------------------------------------------
}
