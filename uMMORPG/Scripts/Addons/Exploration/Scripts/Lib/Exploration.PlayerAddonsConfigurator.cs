using UnityEngine;
using Mirror;

// PLAYER
public partial class PlayerAddonsConfigurator
{
    [HideInInspector] public Exploration_Area myExploration;

    public readonly SyncList<string> exploredAreas = new SyncList<string>();

    protected double fTimerCache;
    protected double fTimerInterval = 10f;


#if _iMMOEXPLORATION
    private void OnStartLocalPlayer_Exploration()
    {
        if (!isServer && !isClient) return;
#if _SERVER
        // player.combat.onKilledEnemy.AddListener(QuestsOnKilled);
#endif

#if _CLIENT
        //player.inventory.onInventoryChanged.AddListener(CheckCompletionQuest); //! For auto quest gather problem numer refresh
#if MIRROR_90_OR_NEWER
        exploredAreas.OnChange += OnExploredAreaUpdated;
#else
#pragma warning disable CS0618
        // TODO : need find other method for this !
        exploredAreas.Callback += OnExploredAreaUpdated;
#pragma warning restore
#endif
#endif
    }
#endif

#if _CLIENT
    // -----------------------------------------------------------------------------------
    // OnHarvestingProfessionUpdated
    // @Client
    // -----------------------------------------------------------------------------------
#if MIRROR_90_OR_NEWER
    void OnExploredAreaUpdated(SyncList<string>.Operation op, int index, string oldIvalue)
#else
    void OnExploredAreaUpdated(SyncList<string>.Operation op, int index, string oldIvalue, string newValue)
#endif
    {
        //UIQuests.TriggerEvent();
        //UIQuestTracker.TriggerEvent();
    }
#endif

    // -----------------------------------------------------------------------------------
    // ExploreArea
    // -----------------------------------------------------------------------------------
#if _SERVER
    [ServerCallback]
    public void ExploreArea()
    {
        if (myExploration && myExploration.explorationRequirements.isActive && myExploration.explorationRequirements.checkRequirements(player))
        {
            // -- explore the area
            if (!exploredAreas.Contains(myExploration.name))
            {
                exploredAreas.Add(myExploration.name);

                myExploration.explorationRewards.gainRewards(player);

                var msg = myExploration.explorePopup.message + myExploration.name;
                player.Tools_ShowPopup(msg, myExploration.explorePopup.iconId, myExploration.explorePopup.soundId);
                player.Tools_MinimapSceneText(myExploration.name);
                fTimerCache = NetworkTime.time + fTimerInterval;

            }
            // -- show notice if already explored
            else if (myExploration.noticeOnEnter)
            {
                if (NetworkTime.time <= fTimerCache) return;
                var msg = myExploration.enterPopup.message + myExploration.name;
                player.Tools_ShowPopup(msg, myExploration.enterPopup.iconId, myExploration.enterPopup.soundId);
                player.Tools_MinimapSceneText(myExploration.name);
                fTimerCache = NetworkTime.time + fTimerInterval;
            }


#if _iMMOQUESTS
            // Require visited Area for update Quests
            player.playerExtendedQuest.IncreaseExplorationCounterFor(myExploration.name);
#endif
        }
    }
#endif
    // -----------------------------------------------------------------------------------
    // HasExploredArea
    // -----------------------------------------------------------------------------------
    public bool HasExploredArea(Exploration_Area simpleExplorationArea)
    {
        return exploredAreas.Contains(simpleExplorationArea.name);
    }
}