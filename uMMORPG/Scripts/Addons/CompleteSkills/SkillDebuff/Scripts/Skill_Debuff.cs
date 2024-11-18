using UnityEngine;

// SKILL DEBUFF

[CreateAssetMenu(menuName = "ADDON/Skills/Target/Debuff", order = 999)]
public class Skill_Debuff : HealSkill
{
    [Header("[-=-[ Skill Debuff ]-=-]")]
    [Tooltip("% Chance to remove the Buff from the target")]
    public LinearFloat successChance;

    public BuffSkill[] removeBuffs;

    [Header("[-=-[ New Buff on Target? ]-=-]")]
    public BuffSkill applyBuff;

    public LinearInt buffLevel;
    public LinearFloat buffChance;

    [Header("[-=-[ Targeting ]-=-]")]
    public bool canHealSelf = true;

    public bool canHealOthers = false;

    // -----------------------------------------------------------------------------------
    // CorrectedTarget
    // -----------------------------------------------------------------------------------
    private Entity CorrectedTarget(Entity caster)
    {
        // targeting nothing? then try to cast on self
        if (caster.target == null)
            return canHealSelf ? caster : null;

        // targeting self?
        if (caster.target == caster)
            return canHealSelf ? caster : null;

        // targeting someone of same type? buff them or self
        if (caster.target.GetType() == caster.GetType())
        {
            if (canHealOthers)
                return caster.target;
            else if (canHealSelf)
                return caster;
            else
                return null;
        }
        else if (caster.target is Monster)
        {
            if (canHealOthers)
                return caster.target;
        }

        // no valid target? try to cast on self or don't cast at all
        return canHealSelf ? caster : null;
    }

    // -----------------------------------------------------------------------------------
    // CheckTarget
    // -----------------------------------------------------------------------------------
    public override bool CheckTarget(Entity caster)
    {
        // correct the target
        caster.target = CorrectedTarget(caster);

        // can only buff the target if it's alive
        return caster.target != null && caster.target.isAlive;
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
        // target still around?
        if (caster.target != null)
        {
            destination = caster.target.collider.ClosestPointOnBounds(caster.transform.position);
            return Utils.ClosestDistance(caster, caster.target) <= castRange.Get(skillLevel);
        }
        destination = caster.transform.position;
        return false;
    }

    // -----------------------------------------------------------------------------------
    //
    // -----------------------------------------------------------------------------------
#if _iMMO2D
    public override void Apply(Entity caster, int skillLevel, Vector2 target)
#else
    public override void Apply(Entity caster, int skillLevel)
#endif
    {
#if _SERVER
        // apply only to alive people
        if (caster.target != null && caster.target.isAlive)
        {
            foreach (BuffSkill removeBuff in removeBuffs)
                caster.target.Tools_RemoveBuff(removeBuff, successChance.Get(skillLevel));

            caster.target.health.current += healsHealth.Get(skillLevel);
            caster.target.mana.current += healsMana.Get(skillLevel);

            caster.target.Tools_ApplyBuff(applyBuff, buffLevel.Get(skillLevel), buffChance.Get(skillLevel));

            SpawnEffect(caster, caster.target);
        }
#endif
    }
    // -----------------------------------------------------------------------------------
}
