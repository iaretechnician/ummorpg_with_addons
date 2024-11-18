using UnityEngine;

// SKILL PARTY LEADER BUFF

[CreateAssetMenu(menuName = "ADDON/Item/Item Party Teleport", order = 999)]
public class Item_PartyTeleport : UsableItem
{
    [Header("[-=-[ Party Teleport]-=-]")]
    [Tooltip("[Optional] Members must be within distance to caster in order to teleport (0 for unlimited distance)")]
    public float maxDistanceToCaster;

    [Tooltip("[Required] GameObject prefab with coordinates OR off scene coordinates (requires Network Zones AddOn)")]
    public Tools_TeleportationTarget teleportationTarget;

    [Tooltip("This will ignore the teleport Location and choose the nearest spawn point instead")]
    public bool teleportToClosestSpawnpoint;

    [Tooltip("Decrease amount by how many each use (can be 0)?")]
    public int decreaseAmount = 1;

#if _SERVER
    // -----------------------------------------------------------------------------------
    // Apply
    // -----------------------------------------------------------------------------------
    public override void Use(Player player, int inventoryIndex)
    {
        ItemSlot slot = player.inventory.slots[inventoryIndex];

        // -- Only activate if enough charges left
        if (decreaseAmount == 0 || slot.amount >= decreaseAmount)
        {
            if (player.party.InParty())
            {
                // always call base function too
                base.Use(player, inventoryIndex);

                foreach (string member in (player.party.party.members))
                {
                    Player plyr = UMMO_Tools.FindOnlinePlayerByName(member);

                    // -- Teleport everybody but not the caster
                    if (plyr != null && plyr != player)
                    {
                        // -- Check Distance

                        if (maxDistanceToCaster <= 0 || Utils.ClosestDistance(player, plyr) <= maxDistanceToCaster || member == player.party.party.master)
                        {
                            // -- Determine Teleportation Target
                            if (teleportToClosestSpawnpoint)
                            {
                                Transform target = NetworkManagerMMO.GetNearestStartPosition(plyr.transform.position);
                                plyr.Tools_Warp(target.position);
                            }
                            else
                            {
                                teleportationTarget.OnTeleport(plyr);
                            }
                        }

                        plyr = null;
                    }
                }

                // decrease amount
                slot.DecreaseAmount(decreaseAmount);
                player.inventory.slots[inventoryIndex] = slot;

                // -- Teleport the caster now
                if (teleportToClosestSpawnpoint)
                {
                    Transform target = NetworkManagerMMO.GetNearestStartPosition(player.transform.position);
                    player.Tools_Warp(target.position);
                }
                else
                {
                    teleportationTarget.OnTeleport(player);
                }
            }
        }
    }
#endif
    // -----------------------------------------------------------------------------------
}
