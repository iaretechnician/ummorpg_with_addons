using Mirror;
using UnityEngine;

//using UnityEditor;

[HideInInspector]
public partial class PlayerAddonsConfigurator : NetworkBehaviour
{
#if _iMMOTOOLS

    [Header("Component")]
    public Player player;
    public PlayerInventory inventory;
    public PlayerEquipment playerEquipment;
    public PlayerExperience playerExperience;
    public Health health;
    public Combat combat;

#if _CLIENT
    // -----------------------------------------------------------------------------------
    // OnStartClient
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Client]
    public override void OnStartClient()
    {
        Utils.InvokeMany(typeof(PlayerAddonsConfigurator), this, "OnStartClient_");
    }

    // -----------------------------------------------------------------------------------
    // OnStartLocalPlayer
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Client]
    public override void OnStartLocalPlayer()
    {
        Utils.InvokeMany(typeof(PlayerAddonsConfigurator), this, "OnStartLocalPlayer_");
    }
#endif

    // -----------------------------------------------------------------------------------
    // OnStartServer
    // @Server
    // -----------------------------------------------------------------------------------

#if _SERVER
    [Server]
    public override void OnStartServer()
    {
        Utils.InvokeMany(typeof(PlayerAddonsConfigurator), this, "OnStartServer_");
    }
#endif



    // -----------------------------------------------------------------------------------
    // Start
    // @Client && @Server
    // -----------------------------------------------------------------------------------
    public void Start()
    {
        //load up some things!
        LoadUpAllComponents();

        Utils.InvokeMany(typeof(PlayerAddonsConfigurator), this, "Start_");
    }

    public void LoadUpAllComponents() 
    {
        if (player == null) player = GetComponent<Player>();
        if (inventory == null) inventory = GetComponent<PlayerInventory>();
        if (health == null) health = GetComponent<Health>();
        if (playerEquipment == null) playerEquipment = GetComponent<PlayerEquipment>();
        if (combat == null) combat = GetComponent<Combat>();
        if (playerExperience == null) playerExperience = GetComponent<PlayerExperience>();
    }

    // -----------------------------------------------------------------------------------
    // Start
    // @Client && @Server
    // -----------------------------------------------------------------------------------
    private void Update()
    {
#if _SERVER && _iMMOFRIENDS
        Update_Friendship();
#endif

#if _iMMOBUILDSYSTEM
        Update_RTS_BuildSystem();
#endif
    }

    // -----------------------------------------------------------------------------------
    // Cmd_ShowAnimation
    // @Client && @Server
    // -----------------------------------------------------------------------------------
    private void LateUpdate()
    {
#if _CLIENT && _iMMOCHEST
        LateUpdate_LootCrate();
#endif

#if _CLIENT && _iMMOCHAIRS
        LateUpdate_Chair();
#endif

#if _CLIENT && _iMMODOORS
        LateUpdate_Doors();
#endif

#if _iMMOBUILDSYSTEM
        LateUpdate_RTS_BuildSystem();
#endif

    }

#endif
}