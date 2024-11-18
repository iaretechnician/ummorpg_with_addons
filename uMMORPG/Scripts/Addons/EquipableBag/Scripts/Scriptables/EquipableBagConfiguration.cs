using UnityEngine;

[CreateAssetMenu(fileName = "Equipable Bag", menuName = "ADDON/Templates/Equipable Bag Configuration", order = 999)]
public class EquipableBagConfiguration : ScriptableObject
{
    [Header("[-=-[ Default Inventory Size ]-=-]")]
    public int defaultInventorySize = 30;
}
