using Mirror;
using UnityEngine;

// BINDPOINT AREA
#if _iMMO2D
[RequireComponent(typeof(CircleCollider2D))]
#else
[RequireComponent(typeof(SphereCollider))]
#endif
public class Area_Bindpoint : NetworkBehaviour
{
    [Header("[-=-[ Bindpoint ]-=-]")]
    public Transform bindpoint;

    [Header("[-=-[ Popups ]-=-]")]
    public Tools_PopupClass enterPopup;

    [Header("[-=-[ Editor ]-=-]")]
    public Color gizmoColor = new Color(0, 1, 1, 0.25f);
    public Color gizmoWireColor = new Color(1, 1, 1, 0.8f);

    // -----------------------------------------------------------------------------------
    // OnDrawGizmos
    // @Editor
    // -----------------------------------------------------------------------------------
    private void OnDrawGizmos()
    {
#if !_iMMO2D
        SphereCollider collider = GetComponent<SphereCollider>();

        // we need to set the gizmo matrix for proper scale & rotation
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(collider.center, collider.radius);
        Gizmos.color = gizmoWireColor;
        Gizmos.DrawWireSphere(collider.center, collider.radius);
        Gizmos.matrix = Matrix4x4.identity;
#endif
    }

    // -------------------------------------------------------------------------------
    // OnTriggerEnter
    // -------------------------------------------------------------------------------
    [ServerCallback]
#if _iMMO2D
    private void OnTriggerEnter2D(Collider2D co)
#else
    private void OnTriggerEnter(Collider co)
#endif
    {
        Player player = co.GetComponentInParent<Player>();
        if (player && bindpoint != null && player.playerAddonsConfigurator.MyBindpoint.name != bindpoint.gameObject.name)
        {
            player.playerAddonsConfigurator.SetBindpointFromArea(bindpoint.gameObject.name, bindpoint.position.x, bindpoint.position.y, bindpoint.position.z);
            player.Tools_ShowPopup(enterPopup.message, enterPopup.iconId, enterPopup.soundId);
        }
    }

    // -------------------------------------------------------------------------------
}