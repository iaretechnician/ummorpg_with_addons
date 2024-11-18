// Setup our item rarity tooltip so the item name is the same color as border.
using System.Text;

public partial struct Item
{
    private void ToolTip_ItemRarity(StringBuilder tip)
    {
        string rarityColor = "";
#if _iMMOITEMLEVELUP
        ScriptableItem.ItemRarity raritySelect = (equipmentLevel > 0 && ((EquipmentItem)data).enableLevelUp) ? ((EquipmentItem)data).LevelUpParameters[equipmentLevel - 1].equipmentLevelUpModifier.rarity : data.rarity;
#else
        ScriptableItem.ItemRarity raritySelect = data.rarity;
#endif
        switch (raritySelect)
        {
            case ScriptableItem.ItemRarity.Poor:
                rarityColor = RarityColor.SetratityColorHtml(ScriptableItem.ItemRarity.Poor.ToString());
                break;

            case ScriptableItem.ItemRarity.Common:
                rarityColor = RarityColor.SetratityColorHtml(ScriptableItem.ItemRarity.Common.ToString());
                break;

            case ScriptableItem.ItemRarity.Uncommon:
                rarityColor = RarityColor.SetratityColorHtml(ScriptableItem.ItemRarity.Uncommon.ToString());
                break;

            case ScriptableItem.ItemRarity.Rare:
                rarityColor = RarityColor.SetratityColorHtml(ScriptableItem.ItemRarity.Rare.ToString());
                break;

            case ScriptableItem.ItemRarity.Epic:
                rarityColor = RarityColor.SetratityColorHtml(ScriptableItem.ItemRarity.Epic.ToString());
                break;

            case ScriptableItem.ItemRarity.Legendary:
                rarityColor = RarityColor.SetratityColorHtml(ScriptableItem.ItemRarity.Legendary.ToString());
                break;
        }

        tip.Replace(name, "<b><color=#" + rarityColor + ">" + name + "</color></b>" );
    }
}