using Mirror;
using UnityEngine;

// ADMINISTRATION - PLAYER

public partial class Player
{
    [Header("Component : Player Admin Chat Command")]
    public PlayerAdminChatCommand playerAdminChatCommand;
    [HideInInspector, SyncVar] public int AdminLevel = 0;
}