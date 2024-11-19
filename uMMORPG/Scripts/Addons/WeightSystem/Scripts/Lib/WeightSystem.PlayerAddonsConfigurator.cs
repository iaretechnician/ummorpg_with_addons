using Mirror;
using UnityEngine;

public partial class PlayerAddonsConfigurator
{
    [Header("[-=-[ Weight System ]-=-]")]
    public WeightSystemConfiguration weightSystemConfiguration;

#if _iMMOWEIGHTSYSTEM
    protected int totalWeight;
    protected int maxWeight;

#if _CLIENT && _iMMOWEIGHTSYSTEM
    public void OnStartLocalPlayer_WeightSysten()
    {
        if (playerEquipment != null && player.inventory != null)
        {
            playerEquipment.onEquipmentChanged.AddListener(Cmd_CalculateWeight);
            player.inventory.onInventoryChanged.AddListener(Cmd_CalculateWeight);
            Cmd_CalculateWeight();
        }
        else
        {
            Debug.LogWarning("PlayerEquipment and/or PlayerInnventory in component PlayerWeightSystem is not assigned in " + player.className + " prefab !");
        }
    }
#endif

    [Command]
    public void Cmd_CalculateWeight()
    {
#if _SERVER
        UpdateEvent_WeightSystem();
#endif
    }

#if _SERVER
    // -----------------------------------------------------------------------------------
    // UpdateEvent
    // @Server
    // -----------------------------------------------------------------------------------
    [ServerCallback]
    public void UpdateEvent_WeightSystem()
    {
        if (weightSystemConfiguration.burdenedBuff == null) return;
        {
            // -- calculate weight
            CalculateWeight();

            // -- check burdened
            int burdenLevel = IsBurdened();

            // -- apply or remove burdened
            if (burdenLevel > 0)
                player.skills.AddOrRefreshBuff(new Buff(weightSystemConfiguration.burdenedBuff, burdenLevel));
            else
                player.Tools_RemoveBuff(weightSystemConfiguration.burdenedBuff);
        }

    }


    // -----------------------------------------------------------------------------------
    // CalculateWeight
    // -----------------------------------------------------------------------------------

    protected void CalculateWeight()
    {
        totalWeight = 0;

#if _iMMOATTRIBUTES
        if (weightSystemConfiguration.weightAttribute != null)
        {
            // Linq is HEAVY on GC and performance
            foreach (Attribute attrib in player.playerAttribute.Attributes)
            {
                if(attrib.name == weightSystemConfiguration.weightAttribute.name)
                {
                    maxWeight = ((weightSystemConfiguration.maxWeight + (attrib.points + player.playerAttribute.calculateBonusAttribute(attrib))) * weightSystemConfiguration.carryPerPoint);
                    break;
                }
            }        
        }
#else
        maxWeight = (weightSystemConfiguration.maxWeight * weightSystemConfiguration.carryPerPoint);
#endif

        for (int i = 0; i < inventory.slots.Count; ++i)
        {
            ItemSlot slot = inventory.slots[i];
            if (slot.amount > 0)
                totalWeight += slot.item.data.weight * slot.amount;
        }
        if (!weightSystemConfiguration.equipmentNotIncluded)
        {
            for (int i = 0; i < player.equipment.slots.Count; ++i)
            {
                ItemSlot slot = player.equipment.slots[i];
                if (slot.amount > 0)
                    totalWeight += slot.item.data.weight * slot.amount;
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // IsBurdened
    // -----------------------------------------------------------------------------------
    protected int IsBurdened()
    {
        if (totalWeight <= maxWeight)
            return 0;
        else
            return Mathf.Min((int)totalWeight / maxWeight, weightSystemConfiguration.maxBurdenLevel);
    }
#endif
#endif
    // -----------------------------------------------------------------------------------
}
