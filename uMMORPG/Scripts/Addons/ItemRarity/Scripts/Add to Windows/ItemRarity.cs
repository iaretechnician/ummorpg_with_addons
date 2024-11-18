using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemRarity : MonoBehaviour
{
    public enum SlotType { Equipment, Inventory, Loot, PlayerTrading, NpcTrading, ItemMall, Crafting }

    public SlotType slotType;

    //private int selfIndex, targetIndex;
    private Player player;
    private ItemSlot itemSlot;
    private ScriptableItem scriptItem;
    private RaritySlot raritySlot;
    private UIShowToolTip tooltip;


    // Assign our componenet based on the slot type.
    private void LateUpdate()
    {
        player = Player.localPlayer;
        if (player != null)
            switch (slotType)
            {

                case SlotType.Loot:
                    if (player.target != null && player.target.health.current <= 0)
                    {
                        UILoot lootContent = GetComponentInParent<UILoot>();
                        List<ItemSlot> items = ((Monster)player.target).inventory.slots.Where(slot => slot.amount > 0).ToList();
                        UIUtils.BalancePrefabs(lootContent.itemSlotPrefab.gameObject, items.Count, lootContent.content);

                        // refresh all valid items
                        for (int i = 0; i < items.Count; ++i)
                        {
                            UILootSlot slot = lootContent.content.GetChild(i).GetComponent<UILootSlot>();
                            slot.dragAndDropable.name = i.ToString(); // drag and drop index
                            int itemIndex = ((Monster)player.target).inventory.slots.FindIndex(
                                // note: .Equals because name AND dynamic variables matter (petLevel etc.)
                                itemSlot => itemSlot.amount > 0 && itemSlot.item.Equals(items[i].item)
                            );

                            // refresh
                            raritySlot = slot.GetComponent<RaritySlot>();
                            tooltip = slot.tooltip;
                            slot.dragAndDropable.name = i.ToString(); // drag and drop slot
                            itemSlot = items[i];
                            raritySlot.rarityOutline.color = RarityColor.SetRarityColorResult(itemSlot.item.data);
                        }
                    }
                    break;

                case SlotType.ItemMall:
                    UIItemMall mallContents = GetComponentInParent<UIItemMall>();
                    if (mallContents.panel.activeSelf)
                    {
                        ScriptableItem[] items = player.itemMall.config.categories[mallContents.currentCategory].items;
                        UIUtils.BalancePrefabs(mallContents.itemSlotPrefab.gameObject, items.Length, mallContents.itemContent);

                        int lastIMCount = 0;
                        int currentPage = 0;
                        if (lastIMCount != items.Length || currentPage != mallContents.currentCategory)
                            for (int i = 0; i < items.Length; ++i)
                            {
                                lastIMCount = items.Length;
                                currentPage = mallContents.currentCategory;
                                UIItemMallSlot slot = mallContents.itemContent.GetChild(i).GetComponent<UIItemMallSlot>();
                                raritySlot = slot.GetComponent<RaritySlot>();
                                tooltip = slot.tooltip;
                                scriptItem = items[i];
                                raritySlot.rarityOutline.color = RarityColor.SetRarityColorResult(scriptItem);
                            }
                    }
                    break;


            }
    }
}