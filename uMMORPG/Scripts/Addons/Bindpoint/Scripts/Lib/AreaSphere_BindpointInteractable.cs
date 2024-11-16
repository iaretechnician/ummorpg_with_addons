using Mirror;
using UnityEngine;

// BINDPOINT AREA
#if _iMMO2D
[RequireComponent(typeof(CircleCollider2D))]
#else
[RequireComponent(typeof(SphereCollider))]
#endif
public class AreaSphere_BindpointInteractable : Tools_InteractableAreaSphere
{
    [Header("[-=-[ Bindpoint ]-=-]")]
    public Transform bindpoint;

    [Header("[-=-[ Popups ]-=-]")]
    public Tools_PopupClass enterPopup;
    
    [ClientCallback]
#if _iMMO2D
    private void OnTriggerEnter2D(Collider2D co)
#else
    private void OnTriggerEnter(Collider co)
#endif
    {
        Player player = co.GetComponentInParent<Player>();
        if (player != null && player == Player.localPlayer && player.playerAddonsConfigurator.MyBindpoint.name != bindpoint.gameObject.name)
        {
            if ((!interactionRequirements.hasRequirements() && !interactionRequirements.requierementCost.HasCosts()) || automaticActivation)
                ConfirmAccess();
            else
                ShowAccessRequirementsUI();
        }
    }
    // -----------------------------------------------------------------------------------
    // OnInteractClient
    // @Client
    // -----------------------------------------------------------------------------------
    [ClientCallback]
    public override void OnInteractClient(Player player)
    {
        if (bindpoint != null && player.playerAddonsConfigurator.MyBindpoint.name != bindpoint.gameObject.name)
            player.Tools_ClientShowPopup(enterPopup.message, enterPopup.iconId, enterPopup.soundId);
    }

    // -----------------------------------------------------------------------------------
    // OnInteractServer
    // @Server
    // -----------------------------------------------------------------------------------
    [ServerCallback]
    public override void OnInteractServer(Player player)
    {
        if (bindpoint != null && player.playerAddonsConfigurator.MyBindpoint.name != bindpoint.gameObject.name)
            player.playerAddonsConfigurator.SetBindpointFromArea(bindpoint.gameObject.name, bindpoint.position.x, bindpoint.position.y, bindpoint.position.z);
    }

    // -------------------------------------------------------------------------------
}