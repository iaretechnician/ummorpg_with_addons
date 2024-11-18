
using Mirror;
using UnityEngine;

// HAZARD FLOOR
#if _iMMO2D
[RequireComponent(typeof(CircleCollider2D))]
#else
[RequireComponent(typeof(SphereCollider))]
#endif
public class AreaHazardFloor : NetworkBehaviour
{
#if _SERVER || UNITY_EDITOR
    [Header("[-=-[ Hazard Floor Settings ]-=-]")]
    public HazardBuff[] onEnterBuff;

    public TargetBuffSkill[] onExitBuff;

    public bool showMessage = false;
    [Header("[-=-[ Popups ]-=-]")]
    public Tools_PopupClass enterPopup;

    public Tools_PopupClass exitPopup;

    [Header("[-=-[ Editor ]-=-]")]
    public Color gizmoColor = new(0, 1, 1, 0.25f);

    public Color gizmoWireColor = new(1, 1, 1, 0.8f);

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

    // -----------------------------------------------------------------------------------
    // Start
    // -----------------------------------------------------------------------------------
    public virtual void Start()
    {
        foreach (HazardBuff buff in onEnterBuff)
        {
            if (buff.buff != null)
            {
                buff.protectiveRequirements.setParent(this.gameObject);
            }
        }
    }
#endif
#if _SERVER

    // -------------------------------------------------------------------------------
    // OnTriggerEnter
    // -------------------------------------------------------------------------------
#if _iMMO2D
    private void OnTriggerEnter2D(Collider2D co)
#else
    private void OnTriggerStay(Collider co)
#endif
    {
        Entity entity = co.GetComponentInParent<Entity>();
        if (entity)
        {
            entity.HazardFloorEnter(onEnterBuff);

            if (entity is Player player && showMessage)
                player.Tools_ShowPopup(enterPopup.message, enterPopup.iconId, enterPopup.soundId);
        }
    }

    // -------------------------------------------------------------------------------
    // OnTriggerExit
    // -------------------------------------------------------------------------------
#if _iMMO2D
    private void OnTriggerExit2D(Collider2D co)
#else
    private void OnTriggerExit(Collider co)
#endif
    {
        Entity entity = co.GetComponentInParent<Entity>();
        if (entity)
        {
            entity.HazardFloorLeave(onExitBuff);

            if (entity is Player player && showMessage)
                player.Tools_ShowPopup(exitPopup.message, exitPopup.iconId, exitPopup.soundId);
        }
    }
#endif
    // -------------------------------------------------------------------------------
}
