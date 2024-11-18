using Mirror;
using System;
using UnityEngine;

// SUMMON SKILL

[CreateAssetMenu(menuName = "ADDON/Skills/Self/Summon Pet", order = 999)]
public class Skill_SummonPet : ScriptableSkill
{
    [Serializable]
    public partial class SpawnableEntity
    {
        public Pet petEntity;
        [Tooltip("[Optional] The level of the summoned Minion (Monster only)")]
        public int level;
        public bool samelPlayerLevel;
    }

    [Serializable]
    public struct SpawnInfo
    {
        public SpawnableEntity gameObjects;
    }

    [Header("[-=-[ Summon Skill ]-=-]")]
    public SpawnInfo[] summonableObjectsPerLevel;

    
    // -----------------------------------------------------------------------------------
    // CheckTarget
    // -----------------------------------------------------------------------------------
    public override bool CheckTarget(Entity caster)
    {
        if(caster is Player player && player.petControl.activePet != null)
        {
            return false;
        }
        return true;
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
        destination = caster.transform.position;
        return true;
    }

    // -----------------------------------------------------------------------------------
    // getSpawnInfo
    // -----------------------------------------------------------------------------------
    protected SpawnInfo getSpawnInfo(int skillLevel)
    {
        if (summonableObjectsPerLevel.Length >= skillLevel - 1)
            return summonableObjectsPerLevel[skillLevel - 1];

        return summonableObjectsPerLevel[0];
    }

    // -----------------------------------------------------------------------------------
    // CanActivate
    // -----------------------------------------------------------------------------------
    public bool CanActivate(Entity caster, SpawnInfo spawn)
    {
		return 	(spawn.gameObjects != null && spawn.gameObjects.petEntity != null);
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
        SpawnInfo spawn = getSpawnInfo(skillLevel);

        // -- General Activation check
        if (CanActivate(caster, spawn))
        {
            if (skillLevel != -1)
            {
                float distance = 1f;

                Vector2 circle2D = UnityEngine.Random.insideUnitCircle * distance;
                Vector3 position = caster.transform.position + new Vector3(circle2D.x, 0, circle2D.y);
                GameObject go = Instantiate(spawn.gameObjects.petEntity.gameObject, position, Quaternion.identity);


                go.name = spawn.gameObjects.petEntity.name; // avoid "(Clone)"


                Pet pet = go.GetComponent<Pet>();
                Player player = (Player)caster;
                if (pet)
                {
                    pet.owner = (Player)caster;
                    pet.level.current = (spawn.gameObjects.samelPlayerLevel) ? pet.owner.level.current : spawn.gameObjects.level;
                    pet.experience.current = 1;
                    pet.health.current = pet.health.max;
#if _iMMOPVP
                    pet.SetRealm(player.Realm, player.Ally);
#endif
                }
                NetworkServer.Spawn(go, player.connectionToClient);
                player.petControl.activePet = pet; // set syncvar after spawning
                player.petControl.isSkillPet = true; 
            }
        }
#endif
    }
    // -----------------------------------------------------------------------------------
}
