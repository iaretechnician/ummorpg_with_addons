using UnityEngine;

// Sets our new hotkeys for equipment.
public partial class UIEquipment : MonoBehaviour
{
    // Set our hotkey based on the players selection.
    public void SetHotkey(KeyCode newHotkey)
    {
        hotKey = newHotkey;
    }
}