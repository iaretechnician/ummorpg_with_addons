﻿using UnityEngine;
using Mirror;

[CreateAssetMenu(menuName = "ADDON/Skills/Target/Backstab", order = 999)]
public class SkillBackstab : DamageSkill
{
    public float teleportDistanceBehind = 1f;

    public override bool CheckTarget(Entity caster)
    {
        // target exists, alive, not self, oktype?
        return caster.target != null && caster.CanAttack(caster.target);
    }

#if _iMMO2D
    public override bool CheckDistance(Entity caster, int skillLevel, out Vector2 destination)
#else
    public override bool CheckDistance(Entity caster, int skillLevel, out Vector3 destination)
#endif
    {
        // target still around?
        if (caster.target != null)
        {
            destination = caster.target.collider.ClosestPoint(caster.transform.position);
            return Utils.ClosestDistance(caster, caster.target) <= castRange.Get(skillLevel);
        }
        destination = caster.transform.position;
        return false;
    }

#if _iMMO2D
    public override void Apply(Entity caster, int skillLevel, Vector2 target)
#else
    public override void Apply(Entity caster, int skillLevel)
#endif
    {
#if _SERVER
        Vector3 teleport = caster.target.transform.position - caster.target.transform.forward * teleportDistanceBehind;
        caster.movement.Warp(teleport);
        caster.RpcBackstabStartTeleport(teleport, caster.target.transform.position);

        // deal damage directly with base damage + skill damage
        caster.combat.DealDamageAt(caster.target,
                            caster.combat.damage + damage.Get(skillLevel),
                            stunChance.Get(skillLevel),
                            stunTime.Get(skillLevel));
#endif
    }
}