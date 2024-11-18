
using UnityEngine;

public static class RarityColor
{
    public static Color SetRarityColorResult(Item slot)
    {
#if _iMMOITEMLEVELUP
        if (slot.equipmentLevel > 0 && ((EquipmentItem)slot.data).enableLevelUp)
            //((EquipmentItem)slot.data).LevelUpParameters[slot.equipmentLevel - 1].equipmentLevelUpModifier.rarity
            //return SetRarityColorResult();
            //
           
        switch (((EquipmentItem)slot.data).LevelUpParameters[slot.equipmentLevel - 1].equipmentLevelUpModifier.rarity)
         {
             case ScriptableItem.ItemRarity.Poor:
                 return SetratityColor(ScriptableItem.ItemRarity.Poor.ToString());

             case ScriptableItem.ItemRarity.Common:
                 return SetratityColor(ScriptableItem.ItemRarity.Common.ToString());

             case ScriptableItem.ItemRarity.Uncommon:
                 return SetratityColor(ScriptableItem.ItemRarity.Uncommon.ToString());

             case ScriptableItem.ItemRarity.Rare:
                 return SetratityColor(ScriptableItem.ItemRarity.Rare.ToString());

             case ScriptableItem.ItemRarity.Epic:
                 return SetratityColor(ScriptableItem.ItemRarity.Epic.ToString());

             case ScriptableItem.ItemRarity.Legendary:
                 return SetratityColor(ScriptableItem.ItemRarity.Legendary.ToString());

             default:
                 return Color.clear;
         }
        else
#endif
            return SetRarityColorResult(slot.data);
    }
    public static Color SetRarityColorResult(ScriptableItem slot)
    {
        switch (slot.rarity)
        {
            case ScriptableItem.ItemRarity.Poor:
                return SetratityColor(ScriptableItem.ItemRarity.Poor.ToString());

            case ScriptableItem.ItemRarity.Common:
                return SetratityColor(ScriptableItem.ItemRarity.Common.ToString());

            case ScriptableItem.ItemRarity.Uncommon:
                return SetratityColor(ScriptableItem.ItemRarity.Uncommon.ToString());

            case ScriptableItem.ItemRarity.Rare:
                return SetratityColor(ScriptableItem.ItemRarity.Rare.ToString());

            case ScriptableItem.ItemRarity.Epic:
                return SetratityColor(ScriptableItem.ItemRarity.Epic.ToString());

            case ScriptableItem.ItemRarity.Legendary:
                return SetratityColor(ScriptableItem.ItemRarity.Legendary.ToString());

            default:
                return Color.clear;
        }
    }

    public static string SetratityColorHtml(string rarity)
    {
        string output = "ScriptableRarityColor";
        int key = output.GetStableHashCode();
        ScriptableRarityColor.All.TryGetValue(key, out var data);

        Color returnColor = Color.gray;
        switch (rarity)
        {
            case "Poor": returnColor = data.poor; break;
            case "Common": returnColor = data.common; break;
            case "Uncommon": returnColor = data.unCommon; break;
            case "Rare": returnColor = data.rare; break;
            case "Epic": returnColor = data.Epic; break;
            case "Legendary": returnColor = data.Legendary; break;
        }
        return ColorUtility.ToHtmlStringRGB(returnColor);
    }
    public static Color SetratityColor(string rarity)
    {
        string output = "ScriptableRarityColor";
        int key = output.GetStableHashCode();
        ScriptableRarityColor.All.TryGetValue(key, out var data);

        Color returnColor = Color.gray;
        switch (rarity)
        {
            case "Poor": returnColor = data.poor; break;
            case "Common": returnColor = data.common; break;
            case "Uncommon": returnColor = data.unCommon; break;
            case "Rare": returnColor = data.rare; break;
            case "Epic": returnColor = data.Epic; break;
            case "Legendary": returnColor = data.Legendary; break;
        }
        return returnColor;
    }
}