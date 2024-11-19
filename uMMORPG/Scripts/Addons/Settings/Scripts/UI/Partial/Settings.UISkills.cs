using UnityEngine;

// Sets our new hotkeys for skills.
public partial class UISkills : MonoBehaviour
{
    // Set our hotkey based on the players selection.
    public void SetHotkey(KeyCode newHotkey)
    {
        hotKey = newHotkey;
    }
}