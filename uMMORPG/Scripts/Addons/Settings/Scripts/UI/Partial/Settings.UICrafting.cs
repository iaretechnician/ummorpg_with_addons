using UnityEngine;

// Sets our new hotkeys for crafting.
public partial class UICrafting : MonoBehaviour
{
    // Set our hotkey based on the players selection.
    public void SetHotkey(KeyCode newHotkey)
    {
        hotKey = newHotkey;
    }
}