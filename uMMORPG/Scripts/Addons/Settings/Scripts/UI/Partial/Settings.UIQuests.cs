using UnityEngine;

// Sets our new hotkeys for quests.
public partial class UIQuests : MonoBehaviour
{
    // Set our hotkey based on the players selection.
    public void SetHotkey(KeyCode newHotkey)
    {
        hotKey = newHotkey;
    }
}