using Mirror;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

// PLAYER

public class PlayerFactions : NetworkBehaviour
{
    public Player player;
    public Health health;

    public UnityEvent onFactionsChanged;
    public GameEvent uiFactionEvent;
    [Header("[-=-[ FACTIONS ]-=-]")]
    public FactionRating[] startingFactions;
    // Declaring a unity event that gets invoked on collision.
    [Header("[-=-[ FACTIONS Modifiers ]-=-]")]
    public FactionModifier[] factionModifiers;

    public string messageFactionModified = "' alignment shifted by ";

    public readonly SyncList<Faction> Factions = new SyncList<Faction>();

#if MIRROR_90_OR_NEWER
    void OnFactionsChanged(SyncList<Faction>.Operation op, int index, Faction oldData)
#else
    void OnFactionsChanged(SyncList<Faction>.Operation op, int index, Faction oldData, Faction newData)
#endif
    {
        onFactionsChanged.Invoke();
        uiFactionEvent.TriggerEvent();
    }

    public override void OnStartClient()
    {
#if MIRROR_90_OR_NEWER
        Factions.OnChange += OnFactionsChanged;
#else
#pragma warning disable CS0618
        Factions.Callback += OnFactionsChanged;
#pragma warning restore CS0618
#endif
    }

    public override void OnStartLocalPlayer()
    {
        health.onEmpty.AddListener(OnDeath);
    }
    [Server]
    private void OnDeath()
    {
#if _SERVER
        if (player.lastAggressor != null)
        {
            if (player.lastAggressor is Player _player)
            {
                foreach (FactionModifier factionModifier in _player.playerFactions.factionModifiers)
                {
                    if (factionModifier.faction != null && factionModifier.amount != 0)
                    {
                        _player.playerFactions.AddFactionRating(factionModifier.faction, factionModifier.amount);
                    }
                }
            }
            if (player.lastAggressor is Pet pet)
            {
                foreach (FactionModifier factionModifier in pet.owner.playerFactions.factionModifiers)
                {
                    if (factionModifier.faction != null && factionModifier.amount != 0)
                    {
                        pet.owner.playerFactions.AddFactionRating(factionModifier.faction, factionModifier.amount);
                    }
                }
            }
        }
#endif
    }


    // -----------------------------------------------------------------------------------
    // AddFactionRating
    // -----------------------------------------------------------------------------------
    public void AddFactionRating(Tmpl_Faction faction, int ratingAmount)
    {
#if _SERVER
        if (faction == null || ratingAmount == 0) return;

        int rating = GetFactionRating(faction);

        if (rating == -99999)
        {
            Faction f = new Faction();
            f.name = faction.name;
            f.rating = ratingAmount;

            Factions.Add(f);
        }
        else
        {
            int idx = Factions.FindIndex(x => x.name == faction.name);
            Faction f = Factions.FirstOrDefault(x => x.name == faction.name);
            f.rating += ratingAmount;
            Factions[idx] = f;
        }

        player.Tools_TargetAddMessage(faction.name + messageFactionModified + ratingAmount.ToString());
#endif
    }

    // -----------------------------------------------------------------------------------
    // GetFactionRating
    // -----------------------------------------------------------------------------------
    public int GetFactionRating(Tmpl_Faction faction)
    {
        if (faction == null) return -99999;
        int idx = Factions.FindIndex(x => x.name == faction.name);
        if (idx != -1)
            return Factions[idx].rating;
        return -99999;
    }

    // -----------------------------------------------------------------------------------
    // CheckFactionRatings
    // -----------------------------------------------------------------------------------
    public bool CheckFactionRatings(FactionRequirement[] factionRequirements, bool requiresAll = false)
    {
        if (factionRequirements.Length == 0) return true;

        bool valid = true;

        foreach (FactionRequirement factionRequirement in factionRequirements)
        {
            valid = CheckFactionRating(factionRequirement);
            if (valid && !requiresAll) return true;
        }

        return valid;
    }

    // -----------------------------------------------------------------------------------
    // CheckFactionRating
    // -----------------------------------------------------------------------------------
    public bool CheckFactionRating(FactionRequirement factionRequirement)
    {
        if (factionRequirement.faction == null) return true;

        int rating = GetFactionRating(factionRequirement.faction);

        return (rating >= factionRequirement.min && rating <= factionRequirement.max);
    }

    // -----------------------------------------------------------------------------------
    // CheckFactionRating
    // -----------------------------------------------------------------------------------
    public bool CheckFactionRating(Quest_Faction factionRequirement)
    {
        if (factionRequirement.faction == null) return true;

        int rating = GetFactionRating(factionRequirement.faction);

        return (rating >= factionRequirement.min);
    }

    // -----------------------------------------------------------------------------------
}
