using Mirror;
using UnityEngine;

[HelpURL("https://mmo-indie.com/addon/Factions")]
public class MonsterFactions : NetworkBehaviour
{
    public Monster monster;
    public Health health;

    [Header("[-=-[ FACTION ]-=-]")]
    public Tmpl_Faction myFaction;

    [Header("[-=-[ FACTIONS Modifiers ]-=-]")]
    public FactionModifier[] factionModifiers;

    private void Start()
    {
        if (monster == null) monster = GetComponent<Monster>();
        if (health == null) health = GetComponent<Health>();
    }

#if _SERVER
    public override void OnStartServer()
    {
        health.onEmpty.AddListener(OnDeath);
    }

    // -----------------------------------------------------------------------------------
    // OnDeath_Factions
    // -----------------------------------------------------------------------------------
    [Server]
    public void OnDeath()
    {
        if (monster.lastAggressor != null)
        {
            if (monster.lastAggressor is Player player)
            {
                foreach (FactionModifier factionModifier in factionModifiers)
                {
                    if (factionModifier.faction != null && factionModifier.amount != 0)
                    {
                        player.playerFactions.AddFactionRating(factionModifier.faction, factionModifier.amount);
                        //Debug.Log("soucis ou pas hez les monstre");
                    }
                }
            }
            if (monster.lastAggressor is Pet pet)
            {
                foreach (FactionModifier factionModifier in factionModifiers)
                {
                    if (factionModifier.faction != null && factionModifier.amount != 0)
                    {
                        pet.owner.playerFactions.AddFactionRating(factionModifier.faction, factionModifier.amount);
                        //Debug.Log("soucis ou pas hez les monstre");
                    }
                }
            }
            /*if (monster.lastAggressor is Player mount)
            {
                foreach (FactionModifier factionModifier in factionModifiers)
                {
                    if (factionModifier.faction != null && factionModifier.amount != 0)
                    {
                        mount.owner.playerFactions.AddFactionRating(factionModifier.faction, factionModifier.amount);
                        Debug.Log("soucis ou pas hez les monstre");
                    }
                }
            }*/

        }
    }

#endif
    // -----------------------------------------------------------------------------------
    // CheckFactionThreshold
    // -----------------------------------------------------------------------------------
    public bool CheckFactionThreshold(Entity entity)
    {
#if _iMMOPVP
        if ( entity is Player && myFaction != null && ((Player)entity).playerFactions.GetFactionRating(myFaction) <= myFaction.aggressiveThreshold)
        {
            return false;
        }
#endif
        return true;
    }

    // -----------------------------------------------------------------------------------
}