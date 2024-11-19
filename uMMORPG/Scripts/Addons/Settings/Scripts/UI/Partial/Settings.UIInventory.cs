using UnityEngine;

// Sets our new hotkeys for inventory.
public partial class UIInventory : MonoBehaviour
{
    // Set our hotkey based on the players selection.
    public void SetHotkey(KeyCode newHotkey)
    {
        hotKey = newHotkey;
    }
}