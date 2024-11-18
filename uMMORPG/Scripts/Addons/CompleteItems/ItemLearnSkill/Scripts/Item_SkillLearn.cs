using System.Linq;
using UnityEngine;

// LEARN SKILL ITEM

[CreateAssetMenu(menuName = "ADDON/Item/Item Learn Skill", order = 999)]
public class Item_SkillLearn : UsableItem
{
    [Header("[-=-[ Learn Skill Item ]-=-]")]
    [Tooltip("[Optional] Required class to use (Player Prefab)")]
    public Player[] allowedClasses;

#if _iMMOPVP

    [Tooltip("[Optional] Only players of these Realms can use")]
    public int requiredRealm;

    public int requiredAlliedRealm;
#endif

    [Tooltip("Skill to be learned - Must be in the player prefabs skill list!")]
    public ScriptableSkill learnedSkill;

    public string learnedMessage = "You just learned: ";
    public string errorMessage = "You cannot learn that skill!";

    [Tooltip("Decrease amount by how many each use (can be 0)?")]
    public int decreaseAmount = 1;

#if _SERVER
    // -----------------------------------------------------------------------------------
    // Use
    // -----------------------------------------------------------------------------------
    public override void Use(Player player, int inventoryIndex)
    {
        ItemSlot slot = player.inventory.slots[inventoryIndex];

        // -- Only activate if the skill is in the player prefabs skill list AND if its not already learned
        if (player.skills.skillTemplates.Any(x => x == learnedSkill) &&
            !((PlayerSkills) player.skills).HasLearned(learnedSkill.name) &&
            (allowedClasses.Length == 0 || player.Tools_checkHasClass(allowedClasses))
#if _iMMOPVP
            && ((requiredRealm == 0 && requiredAlliedRealm == 0) || player.GetAlliedRealms(requiredRealm, requiredAlliedRealm))
#endif
            )
        {
            // -- Only activate if enough charges left
            if (decreaseAmount == 0 || slot.amount >= decreaseAmount)
            {
                // always call base function too
                base.Use(player, inventoryIndex);

                int skillIndex = player.skills.skills.FindIndex(s => s.name == learnedSkill.name);

                Skill skill = player.skills.skills[skillIndex];

                ++skill.level;
                player.skills.skills[skillIndex] = skill;

                player.Tools_TargetAddMessage(learnedMessage + learnedSkill.name);

                // decrease amount
                slot.DecreaseAmount(decreaseAmount);
                player.inventory.slots[inventoryIndex] = slot;
            }
        }
        else
        {
            player.Tools_TargetAddMessage(errorMessage);
        }
    }
#endif
    // -----------------------------------------------------------------------------------
}
