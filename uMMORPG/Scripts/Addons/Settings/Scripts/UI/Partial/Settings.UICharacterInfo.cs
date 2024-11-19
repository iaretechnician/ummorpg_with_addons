using UnityEngine;

// Sets our new hotkeys for character info.
public partial class UI_CharacterInfoAttributes : MonoBehaviour
{
    // Set our hotkey based on the players selection.
    public void SetHotkey(KeyCode newHotkey)
    {
        hotKey = newHotkey;
    }
}

// Sets our new hotkeys for character info.
public partial class UICharacterInfo : MonoBehaviour
{
    // Set our hotkey based on the players selection.
    public void SetHotkey(KeyCode newHotkey)
    {
        hotKey = newHotkey;
    }
}

