// Attach to the prefab for easier component access by the UI Scripts.
// Otherwise we would need slot.GetChild(0).GetComponentInChildren<Text> etc.
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildMemberSlot : MonoBehaviour
{
    public Image onlineStatusImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI rankText;
    public Button promoteButton;
    public Button demoteButton;
    public Button kickButton;
}
