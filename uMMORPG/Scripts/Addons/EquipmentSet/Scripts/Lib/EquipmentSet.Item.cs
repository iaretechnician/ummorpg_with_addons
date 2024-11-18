using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

#if _iMMOEQUIPMENTSETS

public partial struct Item
{
   
    // -----------------------------------------------------------------------------------
    // ValidIndividualSetBonus
    // -----------------------------------------------------------------------------------
    public bool ValidIndividualSetBonus(SyncList<ItemSlot> equipment)
    {
        int counter = 0;

        foreach (var setItem in ((EquipmentItem)data).setItems)
        {
            if (setItem != null)
            {
                counter += equipment.Count(itemSlot => itemSlot.amount > 0 && itemSlot.item.data.name == setItem.name);
            }
        }

        return counter >= ((EquipmentItem)data).setItems.Length;
    }

    // -----------------------------------------------------------------------------------
    // ValidPartialSetBonus
    // -----------------------------------------------------------------------------------
    public bool ValidPartialSetBonus(SyncList<ItemSlot> equipment)
    {
        int counter = 0;

        if (((EquipmentItem)data).equipmentSet != null)
        {
            foreach (var setItem in ((EquipmentItem)data).equipmentSet.setItems)
            {
                if (setItem != null)
                {
                    counter += equipment.Count(itemSlot => itemSlot.amount > 0 && itemSlot.item.data.name == setItem.name);
                }
            }

            if (counter >= ((EquipmentItem)data).equipmentSet.partialSetItemsCount)
            {
                return true;
            }
        }

        return false;
    }


    // -----------------------------------------------------------------------------------
    // ValidCompleteSetBonus
    // -----------------------------------------------------------------------------------
    public bool ValidCompleteSetBonus(SyncList<ItemSlot> equipment)
    {
        if (((EquipmentItem)data).equipmentSet != null)
        {
            int counter = 0;
            HashSet<string> setItemNames = new HashSet<string>();

            foreach (var setItem in ((EquipmentItem)data).equipmentSet.setItems)
            {
                if (setItem != null)
                {
                    setItemNames.Add(setItem.name);
                }
            }

            foreach (var itemSlot in equipment)
            {
                if (itemSlot.amount > 0 && setItemNames.Contains(itemSlot.item.data.name))
                {
                    counter++;
                }
            }

            bool isValid = counter >= setItemNames.Count;

            setItemNames.Clear(); // Libérer la mémoire occupée par le HashSet

            return isValid;
        }

        return false;
    }



    // -----------------------------------------------------------------------------------
    // HasIndividualSetBonus
    // -----------------------------------------------------------------------------------
    public bool HasIndividualSetBonus()
    {
        return data is EquipmentItem item && item.HasIndividualSetBonus;
    }

    // -----------------------------------------------------------------------------------
    // HasPartialSetBonus
    // -----------------------------------------------------------------------------------
    public bool HasPartialSetBonus()
    {
        return data is EquipmentItem item && item.equipmentSet != null && item.equipmentSet.HasPartialSetBonus;
    }

    // -----------------------------------------------------------------------------------
    // HasCompleteSetBonus
    // -----------------------------------------------------------------------------------
    public bool HasCompleteSetBonus()
    {
        return data is EquipmentItem item && item.equipmentSet != null && item.equipmentSet.HasCompleteSetBonus;
    }

    // -----------------------------------------------------------------------------------
    // setBonusAttributeIndividual
    // -----------------------------------------------------------------------------------
#if _iMMOATTRIBUTES

    public int setBonusAttributeIndividual(ItemSlot slot, SyncList<ItemSlot> equipment, Attribute attribute)
    {
        int iPoints = 0;

        // -- Individual Bonus (Applied per Item)
        if (slot.amount > 0 && HasIndividualSetBonus() && ValidIndividualSetBonus(equipment))
        {
            foreach (Tools_AttributeModifier modifier in ((EquipmentItem)data).individualStatModifiers.AttributeModifiers)
            {
                if (modifier.template == attribute.template)
                {
                    iPoints += Convert.ToInt32(attribute.points * modifier.percentBonus);
                    iPoints += modifier.flatBonus;
                }
            }
        }

        return iPoints;
    }

#endif

    // -----------------------------------------------------------------------------------
    // setBonusAttributePartial
    // -----------------------------------------------------------------------------------
#if _iMMOATTRIBUTES

    public int setBonusAttributePartial(ItemSlot slot, SyncList<ItemSlot> equipment, Attribute attribute)
    {
        int iPoints = 0;

        // -- Partial Bonus (Applied Once)
        if (slot.amount > 0 && HasPartialSetBonus() && ValidPartialSetBonus(equipment))
        {
            foreach (Tools_AttributeModifier modifier in ((EquipmentItem)data).equipmentSet.partialStatModifiers.AttributeModifiers)
            {
                if (modifier.template == attribute.template)
                {
                    iPoints += Convert.ToInt32(attribute.points * modifier.percentBonus);
                    iPoints += modifier.flatBonus;
                    break;
                }
            }
        }

        return iPoints;
    }

#endif

    // -----------------------------------------------------------------------------------
    // setBonusAttributeComplete
    // -----------------------------------------------------------------------------------
#if _iMMOATTRIBUTES

    public int setBonusAttributeComplete(ItemSlot slot, SyncList<ItemSlot> equipment, Attribute attribute)
    {
        int iPoints = 0;

        // -- Complete Bonus (Applied Once)
        if (slot.amount > 0 && HasCompleteSetBonus() && ValidCompleteSetBonus(equipment))
        {
            foreach (Tools_AttributeModifier modifier in ((EquipmentItem)data).equipmentSet.completeStatModifiers.AttributeModifiers)
            {
                if (modifier.template == attribute.template)
                {
                    iPoints += Convert.ToInt32(attribute.points * modifier.percentBonus);
                    iPoints += modifier.flatBonus;
                    break;
                }
            }
        }

        return iPoints;
    }

#endif

    // -----------------------------------------------------------------------------------
    // setBonusElementIndividual
    // -----------------------------------------------------------------------------------
#if _iMMOELEMENTS

    public float setBonusElementIndividual(ItemSlot slot, SyncList<ItemSlot> equipment, ElementTemplate element)
    {
        float fPoints = 0;

        // -- Individual Bonus (Applied per Item)
        if (slot.amount > 0 && HasIndividualSetBonus() && ValidIndividualSetBonus(equipment))
        {
            foreach (ElementModifier modifier in ((EquipmentItem)data).individualStatModifiers.elementalResistances)
            {
                if (modifier.template == element)
                {
                    fPoints += modifier.value;
                }
            }
        }

        return fPoints;
    }

#endif

    // -----------------------------------------------------------------------------------
    // setBonusElementPartial
    // -----------------------------------------------------------------------------------
#if _iMMOELEMENTS

    public float setBonusElementPartial(ItemSlot slot, SyncList<ItemSlot> equipment, ElementTemplate element)
    {
        float fPoints = 0;

        // -- Partial Bonus (Applied Once)
        if (slot.amount > 0 && HasPartialSetBonus() && ValidPartialSetBonus(equipment))
        {
            foreach (ElementModifier modifier in ((EquipmentItem)data).equipmentSet.partialStatModifiers.elementalResistances)
            {
                if (modifier.template == element)
                {
                    fPoints += modifier.value;
                    break;
                }
            }
        }

        return fPoints;
    }

#endif

    // -----------------------------------------------------------------------------------
    // setBonusElementComplete
    // -----------------------------------------------------------------------------------
#if _iMMOELEMENTS

    public float setBonusElementComplete(ItemSlot slot, SyncList<ItemSlot> equipment, ElementTemplate element)
    {
        float fPoints = 0;

        // -- Complete Bonus (Applied Once)
        if (slot.amount > 0 && HasCompleteSetBonus() && ValidCompleteSetBonus(equipment))
        {
            foreach (ElementModifier modifier in ((EquipmentItem)data).equipmentSet.completeStatModifiers.elementalResistances)
            {
                if (modifier.template == element)
                {
                    fPoints += modifier.value;
                    break;
                }
            }
        }

        return fPoints;
    }

#endif

    // -----------------------------------------------------------------------------------
    // GETTERS
    // -----------------------------------------------------------------------------------
    #region SetBonus
    public int setIndividualBonusHealth(SyncList<ItemSlot> equipment)
    {
        return ValidIndividualSetBonus(equipment) ? ((EquipmentItem)data).individualStatModifiers.healthBonus : 0;
    }

    public int setPartialBonusHealth(SyncList<ItemSlot> equipment)
    {
        return ValidPartialSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.partialStatModifiers.healthBonus : 0;
    }

    public int setCompleteBonusHealth(SyncList<ItemSlot> equipment)
    {
        return ValidCompleteSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.completeStatModifiers.healthBonus : 0;
    }

    public int setIndividualBonusMana(SyncList<ItemSlot> equipment)
    {
        return ValidIndividualSetBonus(equipment) ? ((EquipmentItem)data).individualStatModifiers.manaBonus : 0;
    }

    public int setPartialBonusMana(SyncList<ItemSlot> equipment)
    {
        return ValidPartialSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.partialStatModifiers.manaBonus : 0;
    }

    public int setCompleteBonusMana(SyncList<ItemSlot> equipment)
    {
        return ValidCompleteSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.completeStatModifiers.manaBonus : 0;
    }

#if _iMMOSTAMINA
    public int setIndividualBonusStamina(SyncList<ItemSlot> equipment)
    {
        return ValidIndividualSetBonus(equipment) ? ((EquipmentItem)data).individualStatModifiers.staminaBonus : 0;
    }

    public int setPartialBonusStamina(SyncList<ItemSlot> equipment)
    {
        return ValidPartialSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.partialStatModifiers.staminaBonus : 0;
    }

    public int setCompleteBonusStamina(SyncList<ItemSlot> equipment)
    {
        return ValidCompleteSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.completeStatModifiers.staminaBonus : 0;
    }
#endif

    public int setIndividualBonusDamage(SyncList<ItemSlot> equipment)
    {
        return ValidIndividualSetBonus(equipment) ? ((EquipmentItem)data).individualStatModifiers.damageBonus : 0;
    }

    public int setPartialBonusDamage(SyncList<ItemSlot> equipment)
    {
        return ValidPartialSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.partialStatModifiers.damageBonus : 0;
    }

    public int setCompleteBonusDamage(SyncList<ItemSlot> equipment)
    {
        return ValidCompleteSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.completeStatModifiers.damageBonus : 0;
    }

    public int setIndividualBonusDefense(SyncList<ItemSlot> equipment)
    {
        return ValidIndividualSetBonus(equipment) ? ((EquipmentItem)data).individualStatModifiers.defenseBonus : 0;
    }

    public int setPartialBonusDefense(SyncList<ItemSlot> equipment)
    {
        return ValidPartialSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.partialStatModifiers.defenseBonus : 0;
    }

    public int setCompleteBonusDefense(SyncList<ItemSlot> equipment)
    {
        return ValidCompleteSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.completeStatModifiers.defenseBonus : 0;
    }

    public float setIndividualBonusBlockChance(SyncList<ItemSlot> equipment)
    {
        return ValidIndividualSetBonus(equipment) ? ((EquipmentItem)data).individualStatModifiers.blockChanceBonus : 0;
    }

    public float setPartialBonusBlockChance(SyncList<ItemSlot> equipment)
    {
        return ValidPartialSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.partialStatModifiers.blockChanceBonus : 0;
    }

    public float setCompleteBonusBlockChance(SyncList<ItemSlot> equipment)
    {
        return ValidCompleteSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.completeStatModifiers.blockChanceBonus : 0;
    }

    public float setIndividualBonusCriticalChance(SyncList<ItemSlot> equipment)
    {
        return ValidIndividualSetBonus(equipment) ? ((EquipmentItem)data).individualStatModifiers.criticalChanceBonus : 0;
    }

    public float setPartialBonusCriticalChance(SyncList<ItemSlot> equipment)
    {
        return ValidPartialSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.partialStatModifiers.criticalChanceBonus : 0;
    }

    public float setCompleteBonusCriticalChance(SyncList<ItemSlot> equipment)
    {
        return ValidCompleteSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.completeStatModifiers.criticalChanceBonus : 0;
    }

#if _iMMOATTRIBUTES

    public float setIndividualBonusBlockFactor(SyncList<ItemSlot> equipment)
    {
        return ValidIndividualSetBonus(equipment) ? ((EquipmentItem)data).individualStatModifiers.bonusBlockFactor : 0;
    }

    public float setPartialBonusBlockFactor(SyncList<ItemSlot> equipment)
    {
        return ValidPartialSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusBlockFactor : 0;
    }

    public float setCompleteBonusBlockFactor(SyncList<ItemSlot> equipment)
    {
        return ValidCompleteSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusBlockFactor : 0;
    }

    public float setIndividualBonusCriticalFactor(SyncList<ItemSlot> equipment)
    {
        return ValidIndividualSetBonus(equipment) ? ((EquipmentItem)data).individualStatModifiers.bonusCriticalFactor : 0;
    }

    public float setPartialBonusCriticalFactor(SyncList<ItemSlot> equipment)
    {
        return ValidPartialSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusCriticalFactor : 0;
    }

    public float setCompleteBonusCriticalFactor(SyncList<ItemSlot> equipment)
    {
        return ValidCompleteSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusCriticalFactor : 0;
    }

    public float setIndividualBonusAccuracy(SyncList<ItemSlot> equipment)
    {
        return ValidIndividualSetBonus(equipment) ? ((EquipmentItem)data).individualStatModifiers.bonusAccuracy : 0;
    }

    public float setPartialBonusAccuracy(SyncList<ItemSlot> equipment)
    {
        return ValidPartialSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusAccuracy : 0;
    }

    public float setCompleteBonusAccuracy(SyncList<ItemSlot> equipment)
    {
        return ValidCompleteSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusAccuracy : 0;
    }

    public float setIndividualBonusResistance(SyncList<ItemSlot> equipment)
    {
        return ValidIndividualSetBonus(equipment) ? ((EquipmentItem)data).individualStatModifiers.bonusResistance : 0;
    }

    public float setPartialBonusResistance(SyncList<ItemSlot> equipment)
    {
        return ValidPartialSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusResistance : 0;
    }

    public float setCompleteBonusResistance(SyncList<ItemSlot> equipment)
    {
        return ValidCompleteSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusResistance : 0;
    }

    public float setIndividualBonusDrainHealthFactor(SyncList<ItemSlot> equipment)
    {
        return ValidIndividualSetBonus(equipment) ? ((EquipmentItem)data).individualStatModifiers.bonusDrainHealthFactor : 0;
    }

    public float setPartialBonusDrainHealthFactor(SyncList<ItemSlot> equipment)
    {
        return ValidPartialSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusDrainHealthFactor : 0;
    }

    public float setCompleteBonusDrainHealthFactor(SyncList<ItemSlot> equipment)
    {
        return ValidCompleteSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusDrainHealthFactor : 0;
    }

    public float setIndividualBonusDrainManaFactor(SyncList<ItemSlot> equipment)
    {
        return ValidIndividualSetBonus(equipment) ? ((EquipmentItem)data).individualStatModifiers.bonusDrainManaFactor : 0;
    }

    public float setPartialBonusDrainManaFactor(SyncList<ItemSlot> equipment)
    {
        return ValidPartialSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusDrainManaFactor : 0;
    }

    public float setCompleteBonusDrainManaFactor(SyncList<ItemSlot> equipment)
    {
        return ValidCompleteSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusDrainManaFactor : 0;
    }

    public float setIndividualBonusReflectDamageFactor(SyncList<ItemSlot> equipment)
    {
        return ValidIndividualSetBonus(equipment) ? ((EquipmentItem)data).individualStatModifiers.bonusReflectDamageFactor : 0;
    }

    public float setPartialBonusReflectDamageFactor(SyncList<ItemSlot> equipment)
    {
        return ValidPartialSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusReflectDamageFactor : 0;
    }

    public float setCompleteBonusReflectDamageFactor(SyncList<ItemSlot> equipment)
    {
        return ValidCompleteSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusReflectDamageFactor : 0;
    }

    public float setIndividualBonusDefenseBreakFactor(SyncList<ItemSlot> equipment)
    {
        return ValidIndividualSetBonus(equipment) ? ((EquipmentItem)data).individualStatModifiers.bonusDefenseBreakFactor : 0;
    }

    public float setPartialBonusDefenseBreakFactor(SyncList<ItemSlot> equipment)
    {
        return ValidPartialSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusDefenseBreakFactor : 0;
    }

    public float setCompleteBonusDefenseBreakFactor(SyncList<ItemSlot> equipment)
    {
        return ValidCompleteSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusDefenseBreakFactor : 0;
    }

    public float setIndividualBonusBlockBreakFactor(SyncList<ItemSlot> equipment)
    {
        return ValidIndividualSetBonus(equipment) ? ((EquipmentItem)data).individualStatModifiers.bonusBlockBreakFactor : 0;
    }

    public float setPartialBonusBlockBreakFactor(SyncList<ItemSlot> equipment)
    {
        return ValidPartialSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusBlockBreakFactor : 0;
    }

    public float setCompleteBonusBlockBreakFactor(SyncList<ItemSlot> equipment)
    {
        return ValidCompleteSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusBlockBreakFactor : 0;
    }

    public float setIndividualBonusCriticalEvasion(SyncList<ItemSlot> equipment)
    {
        return ValidIndividualSetBonus(equipment) ? ((EquipmentItem)data).individualStatModifiers.bonusCriticalEvasion : 0;
    }

    public float setPartialBonusCriticalEvasion(SyncList<ItemSlot> equipment)
    {
        return ValidPartialSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusCriticalEvasion : 0;
    }

    public float setCompleteBonusCriticalEvasion(SyncList<ItemSlot> equipment)
    {
        return ValidCompleteSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusCriticalEvasion : 0;
    }

    public float setIndividualBonusAbsorbHealthFactor(SyncList<ItemSlot> equipment)
    {
        return ValidIndividualSetBonus(equipment) ? ((EquipmentItem)data).individualStatModifiers.bonusAbsorbHealthFactor : 0;
    }

    public float setPartialBonusAbsorbHealthFactor(SyncList<ItemSlot> equipment)
    {
        return ValidPartialSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusAbsorbHealthFactor : 0;
    }

    public float setCompleteBonusAbsorbHealthFactor(SyncList<ItemSlot> equipment)
    {
        return ValidCompleteSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusAbsorbHealthFactor : 0;
    }

    public float setIndividualBonusAbsorbManaFactor(SyncList<ItemSlot> equipment)
    {
        return ValidIndividualSetBonus(equipment) ? ((EquipmentItem)data).individualStatModifiers.bonusAbsorbManaFactor : 0;
    }

    public float setPartialBonusAbsorbManaFactor(SyncList<ItemSlot> equipment)
    {
        return ValidPartialSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.partialStatModifiers.bonusAbsorbManaFactor : 0;
    }

    public float setCompleteBonusAbsorbManaFactor(SyncList<ItemSlot> equipment)
    {
        return ValidCompleteSetBonus(equipment) ? ((EquipmentItem)data).equipmentSet.completeStatModifiers.bonusAbsorbManaFactor : 0;
    }
    
#endif
    #endregion SetBonus
    // -----------------------------------------------------------------------------------
    // TOOLTIP
    // -----------------------------------------------------------------------------------
    private void ToolTip_EquipmentSets(StringBuilder tip)
    {
        if (data is not EquipmentItem) return;

        EquipmentSetTemplate equipmentSet = ((EquipmentItem)data).equipmentSet;
        if (equipmentSet == null) return;

        tip.AppendLine();
        tip.AppendLine("<b>" + equipmentSet.name + "</b>");
        tip.AppendLine("<i>-----------------------------------------------</i>");
        tip.AppendLine();

        tip.AppendLine("<b>-=-List Set Items -=-</b>");
        int equippedItemCount = ComparListItem().Count;
        tip.AppendLine($"{(equipmentSet.setItems.Length - equippedItemCount)} / {equipmentSet.setItems.Length} currently equipped.");

        foreach (var setItem in equipmentSet.setItems)
        {
            if (setItem != null)
            {
                string itemName = setItem.name;
                bool isEquipped = ListEquipmentItem().Contains(itemName);
                tip.AppendLine("* " + (isEquipped ? "<b><color=green>" + itemName + "</color></b>" : itemName));
            }
        }

        // --------------------------------------------------------------------------- Individual Set Bonus
        if (HasIndividualSetBonus())
        {        
            tip.Append("\n");
            tip.Append("<b>-=- Individual Set Bonus -=-</b>\n");
            tip.Append(((EquipmentItem)data).setItems.Length + " / " + ((EquipmentItem)data).setItems.Length + " Item(s) equipped:\n");
            for (int j = 0; j < ((EquipmentItem)data).setItems.Length; ++j)
            {
                if (((EquipmentItem)data).setItems[j] != null) tip.Append("* " + ((EquipmentItem)data).setItems[j].name + "\n");
            }

            Tools_StatModifier _individualStatModifier = ((EquipmentItem)data).individualStatModifiers;

#if _iMMOATTRIBUTES
            foreach (Tools_AttributeModifier modifier in _individualStatModifier.AttributeModifiers)
            {
                tip.Append(modifier.template.name + " Bonus: " + modifier.flatBonus.ToString() + "/ " + Mathf.RoundToInt(modifier.percentBonus * 100).ToString() + "%\n");
            }
#endif

#if _iMMOELEMENTS
            foreach (ElementModifier modifier in _individualStatModifier.elementalResistances)
            {
                tip.Append(modifier.template.name + " Resistance: " + Mathf.RoundToInt(modifier.value * 100).ToString() + "%\n");
            }
#endif


            if (_individualStatModifier.healthBonus != 0) tip.Append("Health Bonus:" + _individualStatModifier.healthBonus.ToString() + "\n");
            if (_individualStatModifier.manaBonus != 0) tip.Append("Mana Bonus:" + _individualStatModifier.manaBonus.ToString() + "\n");
            if (_individualStatModifier.damageBonus != 0) tip.Append("Damage Bonus:" + _individualStatModifier.damageBonus.ToString() + "\n");
            if (_individualStatModifier.defenseBonus != 0) tip.Append("Defense Bonus:" + _individualStatModifier.defenseBonus.ToString() + "\n");

            if (_individualStatModifier.blockChanceBonus != 0) tip.Append("Block Chance Bonus:" + Mathf.RoundToInt(_individualStatModifier.blockChanceBonus * 100).ToString() + "%\n");
            if (_individualStatModifier.criticalChanceBonus != 0) tip.Append("Critical Chance Bonus:" + Mathf.RoundToInt(_individualStatModifier.criticalChanceBonus * 100).ToString() + "%\n");
#if _iMMOATTRIBUTES
            if (_individualStatModifier.bonusBlockFactor != 0) tip.Append("Block Factor Bonus:" + Mathf.RoundToInt(_individualStatModifier.bonusBlockFactor * 100).ToString() + "%\n");
            if (_individualStatModifier.bonusCriticalFactor != 0) tip.Append("Critical Factor Bonus:" + Mathf.RoundToInt(_individualStatModifier.bonusCriticalFactor * 100).ToString() + "%\n");
            if (_individualStatModifier.bonusDrainHealthFactor != 0) tip.Append("Drain Health Bonus:" + Mathf.RoundToInt(_individualStatModifier.bonusDrainHealthFactor * 100).ToString() + "%\n");
            if (_individualStatModifier.bonusDrainManaFactor != 0) tip.Append("Drain Mana Bonus:" + Mathf.RoundToInt(_individualStatModifier.bonusDrainManaFactor * 100).ToString() + "%\n");
            if (_individualStatModifier.bonusReflectDamageFactor != 0) tip.Append("Reflect Damage Bonus:" + Mathf.RoundToInt(_individualStatModifier.bonusReflectDamageFactor * 100).ToString() + "%\n");
            if (_individualStatModifier.bonusDefenseBreakFactor != 0) tip.Append("Defense Break Bonus:" + Mathf.RoundToInt(_individualStatModifier.bonusDefenseBreakFactor * 100).ToString() + "%\n");
            if (_individualStatModifier.bonusBlockBreakFactor != 0) tip.Append("Block Break Bonus:" + Mathf.RoundToInt(_individualStatModifier.bonusBlockBreakFactor * 100).ToString() + "%\n");
            if (_individualStatModifier.bonusCriticalEvasion != 0) tip.Append("Critical Evasion Bonus:" + Mathf.RoundToInt(_individualStatModifier.bonusCriticalEvasion * 100).ToString() + "%\n");
            if (_individualStatModifier.bonusAccuracy != 0) tip.Append("Accuracy Bonus:" + Mathf.RoundToInt(_individualStatModifier.bonusAccuracy * 100).ToString() + "%\n");
            if (_individualStatModifier.bonusResistance != 0) tip.Append("Resistance Bonus:" + Mathf.RoundToInt(_individualStatModifier.bonusResistance * 100).ToString() + "%\n");

            if (_individualStatModifier.bonusAbsorbHealthFactor != 0) tip.Append("Absorb Health Bonus:" + Mathf.RoundToInt(_individualStatModifier.bonusAbsorbHealthFactor * 100).ToString() + "%\n");
            if (_individualStatModifier.bonusAbsorbManaFactor != 0) tip.Append("Absorb Mana Bonus:" + Mathf.RoundToInt(_individualStatModifier.bonusAbsorbManaFactor * 100).ToString() + "%\n");

#endif
        }

        // --------------------------------------------------------------------------- Partial Set Bonus
        if (HasPartialSetBonus())
        {
            tip.Append("\n");
            tip.Append("<b>-=- Partial Set Bonus  "+ ((ComparListItem().Count > 0 && ComparListItem().Count <= (((EquipmentItem)data).equipmentSet.setItems.Length - ((EquipmentItem)data).equipmentSet.partialSetItemsCount)) ? "<b><color=red>(Active)</color></b>" : "") + " -=-</b>\n");
            tip.Append(((EquipmentItem)data).equipmentSet.partialSetItemsCount + " / " + ((EquipmentItem)data).equipmentSet.setItems.Length + " Item(s) equipped.\n");

            Tools_StatModifier _partialStatModifiers = ((EquipmentItem)data).equipmentSet.partialStatModifiers;
#if _iMMOATTRIBUTES
            foreach (Tools_AttributeModifier modifier in _partialStatModifiers.AttributeModifiers)
            {
                tip.Append(modifier.template.name + " Bonus: " + modifier.flatBonus.ToString() + "/ " + Mathf.RoundToInt(modifier.percentBonus * 100).ToString() + "%\n");
            }
#endif

#if _iMMOELEMENTS
            foreach (ElementModifier modifier in _partialStatModifiers.elementalResistances)
            {
                tip.Append(modifier.template.name + " Resistance: " + Mathf.RoundToInt(modifier.value * 100).ToString() + "%\n");
            }
#endif

            if (_partialStatModifiers.healthBonus != 0) tip.Append("Health Bonus:" + _partialStatModifiers.healthBonus.ToString() + "\n");
            if (_partialStatModifiers.manaBonus != 0) tip.Append("Mana Bonus:" + _partialStatModifiers.manaBonus.ToString() + "\n");
            if (_partialStatModifiers.damageBonus != 0) tip.Append("Damage Bonus:" + _partialStatModifiers.damageBonus.ToString() + "\n");
            if (_partialStatModifiers.defenseBonus != 0) tip.Append("Defense Bonus:" + _partialStatModifiers.defenseBonus.ToString() + "\n");

            if (_partialStatModifiers.blockChanceBonus != 0) tip.Append("Block Chance Bonus:" + Mathf.RoundToInt(_partialStatModifiers.blockChanceBonus * 100).ToString() + "%\n");
            if (_partialStatModifiers.criticalChanceBonus != 0) tip.Append("Critical Chance Bonus:" + Mathf.RoundToInt(_partialStatModifiers.criticalChanceBonus * 100).ToString() + "%\n");
#if _iMMOATTRIBUTES
            if (_partialStatModifiers.bonusBlockFactor != 0) tip.Append("<b><color=teal>Block Factor Bonus:</color></b> " + Mathf.RoundToInt(_partialStatModifiers.bonusBlockFactor * 100).ToString() + " %\n");
            if (_partialStatModifiers.bonusCriticalFactor != 0) tip.Append("Critical Factor Bonus:" + Mathf.RoundToInt(_partialStatModifiers.bonusCriticalFactor * 100).ToString() + "%\n");
            if (_partialStatModifiers.bonusDrainHealthFactor != 0) tip.Append("Drain Health Bonus:" + Mathf.RoundToInt(_partialStatModifiers.bonusDrainHealthFactor * 100).ToString() + "%\n");
            if (_partialStatModifiers.bonusDrainManaFactor != 0) tip.Append("Drain Mana Bonus:" + Mathf.RoundToInt(_partialStatModifiers.bonusDrainManaFactor * 100).ToString() + "%\n");
            if (_partialStatModifiers.bonusReflectDamageFactor != 0) tip.Append("Reflect Damage Bonus:" + Mathf.RoundToInt(_partialStatModifiers.bonusReflectDamageFactor * 100).ToString() + "%\n");
            if (_partialStatModifiers.bonusDefenseBreakFactor != 0) tip.Append("Defense Break Bonus:" + Mathf.RoundToInt(_partialStatModifiers.bonusDefenseBreakFactor * 100).ToString() + "%\n");
            if (_partialStatModifiers.bonusBlockBreakFactor != 0) tip.Append("Block Break Bonus:" + Mathf.RoundToInt(_partialStatModifiers.bonusBlockBreakFactor * 100).ToString() + "%\n");
            if (_partialStatModifiers.bonusCriticalEvasion != 0) tip.Append("Critical Evasion Bonus:" + Mathf.RoundToInt(_partialStatModifiers.bonusCriticalEvasion * 100).ToString() + "%\n");
            if (_partialStatModifiers.bonusAccuracy != 0) tip.Append("Accuracy Bonus:" + Mathf.RoundToInt(_partialStatModifiers.bonusAccuracy * 100).ToString() + "%\n");
            if (_partialStatModifiers.bonusResistance != 0) tip.Append("Resistance Bonus:" + Mathf.RoundToInt(_partialStatModifiers.bonusResistance * 100).ToString() + "%\n");

            if (_partialStatModifiers.bonusAbsorbHealthFactor != 0) tip.Append("Absorb Health Bonus:" + Mathf.RoundToInt(_partialStatModifiers.bonusAbsorbHealthFactor * 100).ToString() + "%\n");
            if (_partialStatModifiers.bonusAbsorbManaFactor != 0) tip.Append("Absorb Mana Bonus:" + Mathf.RoundToInt(_partialStatModifiers.bonusAbsorbManaFactor * 100).ToString() + "%\n");

#endif
        }

        // --------------------------------------------------------------------------- Complete Set Bonus
        if (HasCompleteSetBonus())
        {

            tip.Append("\n");
            tip.Append("<b>-=- Complete Set Bonus "+ ((ComparListItem().Count == 0) ? "<b><color=red>(Active)</color></b>" : "")  + "-=-</b>\n");
            tip.Append(((EquipmentItem)data).equipmentSet.setItems.Length + " / " + ((EquipmentItem)data).equipmentSet.setItems.Length + " Item(s) equipped:.\n");

            Tools_StatModifier _completeStatModifiers = ((EquipmentItem)data).equipmentSet.completeStatModifiers;
#if _iMMOATTRIBUTES
            foreach (Tools_AttributeModifier modifier in _completeStatModifiers.AttributeModifiers)
            {
                tip.Append(modifier.template.name + " Bonus: " + modifier.flatBonus.ToString() + "/ " + Mathf.RoundToInt(modifier.percentBonus * 100).ToString() + "%\n");
            }
#endif

#if _iMMOELEMENTS
            foreach (ElementModifier modifier in _completeStatModifiers.elementalResistances)
            {
                tip.Append(modifier.template.name + " Resistance: " + Mathf.RoundToInt(modifier.value * 100).ToString() + "%\n");
            }
#endif

            if (_completeStatModifiers.healthBonus != 0) tip.Append("Health Bonus:" + _completeStatModifiers.healthBonus.ToString() + "\n");
            if (_completeStatModifiers.manaBonus != 0) tip.Append("Mana Bonus:" + _completeStatModifiers.manaBonus.ToString() + "\n");
            if (_completeStatModifiers.damageBonus != 0) tip.Append("Damage Bonus:" + _completeStatModifiers.damageBonus.ToString() + "\n");
            if (_completeStatModifiers.defenseBonus != 0) tip.Append("Defense Bonus:" + _completeStatModifiers.defenseBonus.ToString() + "\n");

            if (_completeStatModifiers.blockChanceBonus != 0) tip.Append("Block Chance Bonus:" + Mathf.RoundToInt(_completeStatModifiers.blockChanceBonus * 100).ToString() + "%\n");
            if (_completeStatModifiers.criticalChanceBonus != 0) tip.Append("Critical Chance Bonus:" + Mathf.RoundToInt(_completeStatModifiers.criticalChanceBonus * 100).ToString() + "%\n");
#if _iMMOATTRIBUTES
            if (_completeStatModifiers.bonusBlockFactor != 0) tip.Append("<b><color=teal>Block Factor Bonus:</color></b> " + Mathf.RoundToInt(_completeStatModifiers.bonusBlockFactor * 100).ToString() + "%\n");
            if (_completeStatModifiers.bonusCriticalFactor != 0) tip.Append("Critical Factor Bonus:" + Mathf.RoundToInt(_completeStatModifiers.bonusCriticalFactor * 100).ToString() + "%\n");
            if (_completeStatModifiers.bonusDrainHealthFactor != 0) tip.Append("Drain Health Bonus:" + Mathf.RoundToInt(_completeStatModifiers.bonusDrainHealthFactor * 100).ToString() + "%\n");
            if (_completeStatModifiers.bonusDrainManaFactor != 0) tip.Append("Drain Mana Bonus:" + Mathf.RoundToInt(_completeStatModifiers.bonusDrainManaFactor * 100).ToString() + "%\n");
            if (_completeStatModifiers.bonusReflectDamageFactor != 0) tip.Append("Reflect Damage Bonus:" + Mathf.RoundToInt(_completeStatModifiers.bonusReflectDamageFactor * 100).ToString() + "%\n");
            if (_completeStatModifiers.bonusDefenseBreakFactor != 0) tip.Append("Defense Break Bonus:" + Mathf.RoundToInt(_completeStatModifiers.bonusDefenseBreakFactor * 100).ToString() + "%\n");
            if (_completeStatModifiers.bonusBlockBreakFactor != 0) tip.Append("Block Break Bonus:" + Mathf.RoundToInt(_completeStatModifiers.bonusBlockBreakFactor * 100).ToString() + "%\n");
            if (_completeStatModifiers.bonusCriticalEvasion != 0) tip.Append("Critical Evasion Bonus:" + Mathf.RoundToInt(_completeStatModifiers.bonusCriticalEvasion * 100).ToString() + "%\n");
            if (_completeStatModifiers.bonusAccuracy != 0) tip.Append("Accuracy Bonus:" + Mathf.RoundToInt(_completeStatModifiers.bonusAccuracy * 100).ToString() + "%\n");
            if (_completeStatModifiers.bonusResistance != 0) tip.Append("Resistance Bonus:" + Mathf.RoundToInt(_completeStatModifiers.bonusResistance * 100).ToString() + "%\n");
            if (_completeStatModifiers.bonusAbsorbHealthFactor != 0) tip.Append("Absorb Health Bonus:" + Mathf.RoundToInt(_completeStatModifiers.bonusAbsorbHealthFactor * 100).ToString() + "%\n");
            if (_completeStatModifiers.bonusAbsorbManaFactor != 0) tip.Append("Absorb Mana Bonus:" + Mathf.RoundToInt(_completeStatModifiers.bonusAbsorbManaFactor * 100).ToString() + "%\n");
#endif
        }
    }


    
    private List<string> ListEquipmentItem()
    {
        List<string> _itemsNameList = new();
        if(_itemsNameList.Count == 0)
        {

            Player p = Player.localPlayer;
            for (int i = 0; i < p.equipment.slots.Count; ++i)
            {
                EquipmentInfo slotInfo = ((PlayerEquipment)p.equipment).slotInfo[i]; // TODO useless ?
                ItemSlot itemSlot = p.equipment.slots[i]; // TODO useless ?
                if (p.equipment.slots[i].amount > 0)
                {
                    ScriptableItem.All.TryGetValue(p.equipment.slots[i].item.name.GetStableHashCode(), out ScriptableItem itemData);
                    if (itemData.name != null && !_itemsNameList.Contains(itemData.name)) _itemsNameList.Add(itemData.name);
                }
            }

        }
        return _itemsNameList;
    }
    private List<string> ListEquimentSet()
    {
        List<string> _itemsNameList = new();
        for (int j = 0; j < ((EquipmentItem)data).equipmentSet.setItems.Length; ++j)
        {
            if (((EquipmentItem)data).equipmentSet.setItems[j] != null)
            {
                string itemName = ((EquipmentItem)data).equipmentSet.setItems[j].name;
                _itemsNameList.Add(itemName);
            }
        }
        return _itemsNameList;
    }

    private List<string> ComparListItem()
    {
        List<string> result = new();
        result.AddRange(ListEquimentSet().Except(ListEquipmentItem(), StringComparer.OrdinalIgnoreCase));
        return result;
    }
}

#endif
