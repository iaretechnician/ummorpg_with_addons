using Mirror;
using System.Collections.Generic;
using UnityEngine;

// ITEM TARGETTED AOE

[CreateAssetMenu(menuName = "ADDON/Item/Item Targetted AOE", order = 999)]
public class Item_TargettedAOE : UsableItem
{
    [Header("[-=-[ Target AOE Item ]-=-]")]
    public OneTimeTargetSkillEffect effect;

    public LinearInt damage = new LinearInt { baseValue = 1 };
    public LinearFloat stunChance; // range [0,1]
    public LinearFloat stunTime; // in seconds
    public int skillLevel;
    public float range;
    public float radius;
    [Range(0, 1)] public float triggerAggroChance;
    public string successMessage = "You damaged: ";
    public bool SpawnEffectOnMainTargetOnly;

    [Header("[-=-[ Apply Buff on Target ]-=-]")]
    public BuffSkill applyBuff;

    public int buffLevel;
    [Range(0, 1)] public float buffChance;

    [Header("[-=-[ Targeting ]-=-]")]
    [Tooltip("[Optional] Changes 'affect' affect into 'not affect' and vice-versa")]
    public bool reverseTargeting;

    [Tooltip("[Optional] Does affect the caster")]
    public bool affectSelf;

    [Tooltip("[Optional] Does affect members of the own party")]
    public bool affectOwnParty;

    [Tooltip("[Optional] Does affect members of the own guild")]
    public bool affectOwnGuild;

    [Tooltip("[Optional] Does affect members of the own realm (requires PVP ZONE AddOn")]
    public bool affectOwnRealm;

    public bool affectPlayers;
    public bool affectNpcs;
    public bool affectMonsters;
    public bool affectPets;

    [Tooltip("Decrease amount by how many each use (can be 0)?")]
    public int decreaseAmount = 1;


    // -----------------------------------------------------------------------------------
    // CheckTarget
    // -----------------------------------------------------------------------------------
    /*public bool CheckTarget(Entity caster)
    {
        return caster.target != null && caster.target != caster;
    }*/

    // -----------------------------------------------------------------------------------
    // CheckDistance
    // -----------------------------------------------------------------------------------
    public bool CheckDistance(Entity caster, int skillLevel, out Vector3 destination)
    {
        // target still around?
        if (caster.target != null)
        {
            destination = caster.target.collider.ClosestPoint(caster.transform.position);
            return Utils.ClosestDistance(caster, caster.target) <= range;
        }
        destination = caster.transform.position;
        return false;
    }

#if _SERVER
    // -----------------------------------------------------------------------------------
    // Use
    // -----------------------------------------------------------------------------------
    public override void Use(Player player, int inventoryIndex)
    {
        List<Entity> targets = new List<Entity>();
        ItemSlot slot = player.inventory.slots[inventoryIndex];

        // -- Only activate if enough charges left
        if (decreaseAmount == 0 || slot.amount >= decreaseAmount)
        {
            // always call base function too
            base.Use(player, inventoryIndex);

            if (SpawnEffectOnMainTargetOnly)
                SpawnEffect(player, player.target);

            targets = player.Tools_GetCorrectedTargetsInSphere(player.target.transform, radius, false, affectSelf, affectOwnParty, affectOwnGuild, affectOwnRealm, reverseTargeting, affectPlayers, affectNpcs, affectMonsters, affectPets);

            foreach (Entity target in targets)
            {
                // deal damage directly with base damage + skill damage
                player.combat.DealDamageAt(target, player.combat.damage + damage.Get(skillLevel), stunChance.Get(skillLevel), stunTime.Get(skillLevel));

                target.Tools_ApplyBuff(applyBuff, buffLevel, buffChance);
                player.Tools_TargetAddMessage(successMessage + target.name);

                if (!SpawnEffectOnMainTargetOnly)
                    SpawnEffect(player, target);

                if (UnityEngine.Random.value <= triggerAggroChance)
                    target.target = player;
            }

            // decrease amount
            slot.DecreaseAmount(decreaseAmount);
            player.inventory.slots[inventoryIndex] = slot;
        }

        targets.Clear();
    }

    // -----------------------------------------------------------------------------------
    // SpawnEffect
    // -----------------------------------------------------------------------------------
    public void SpawnEffect(Entity caster, Entity spawnTarget)
    {
        if (effect != null)
        {
            GameObject go = Instantiate(effect.gameObject, spawnTarget.transform.position, Quaternion.identity);
            go.GetComponent<OneTimeTargetSkillEffect>().caster = caster;
            go.GetComponent<OneTimeTargetSkillEffect>().target = spawnTarget;
            NetworkServer.Spawn(go);
        }
    }
#endif
    // -----------------------------------------------------------------------------------
}
