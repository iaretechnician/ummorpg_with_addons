using Mirror;
using UnityEngine;

public partial class MonsterWorldBossMessage : NetworkBehaviour
{

    public Monster monster;
    public Combat combat;
    public Health health;

    public bool displayMessageInChat = true;
    public bool isWorldBoss;

    [Header("[-=-[ World Boss killed Message ]-=-]")]
    [BoolShowConditional(conditionFieldName: "isWorldBoss", conditionValue: true)]
    public Tools_PopupClass bossKilled = new() { message = "The world boss {0} was killed by {1}" , suffix = "", iconId = 0, soundId = 0 };


    [Header("[-=-[ World Boss Respawn Message ]-=-]")]
    [BoolShowConditional(conditionFieldName: "isWorldBoss", conditionValue: true)]
    public Tools_PopupClass bossRespawn = new() { message = "The world boss {0} is respawned", suffix = "", iconId = 0, soundId = 0 };



#if _SERVER && _iMMOWORLDBOSSMESSAGE
    // ----------------------------------------------------------------------------------
    // Auto Add in Health event
    // ---------------------------------------------------------------------------------- 
    public override void OnStartServer()
    {
        health.onEmpty.AddListener(OnDeath_WorldBossMessage);
    }

    [Server]
    public void OnDeath_WorldBossMessage()
    {
        // This function not included on client
        if (isWorldBoss && Player.onlinePlayers.Count > 0 && monster.lastAggressor != null)
        {
            if (monster.lastAggressor is Player || monster.lastAggressor is Pet || monster.lastAggressor is Mount)
            {
                string lastAgressorName = (monster.lastAggressor is Pet pet) ? pet.owner.name : ((monster.lastAggressor is Mount mount) ? mount.owner.name : monster.lastAggressor.name);

                foreach (Player playerOnline in Player.onlinePlayers.Values)
                {
                    playerOnline.Target_Tools_ShowPopup(playerOnline.connectionToClient, string.Format(bossKilled.message, monster.name, lastAgressorName), bossKilled.iconId, bossKilled.soundId);
                    if (displayMessageInChat)
                        playerOnline.Tools_TargetAddMessage(string.Format(bossKilled.message, monster.name, lastAgressorName));
                }
            }
        }
    }

    [Server]
    public void OnReviveMessage()
    {
        if (isWorldBoss)
        {
            foreach (Player playerOnline in Player.onlinePlayers.Values)
            {
                playerOnline.Target_Tools_ShowPopup(playerOnline.connectionToClient, string.Format(bossRespawn.message, monster.name), bossRespawn.iconId, bossRespawn.soundId);
                if (displayMessageInChat)
                    playerOnline.Tools_TargetAddMessage(string.Format(bossRespawn.message, monster.name));
            }
        }
    }
#endif
}