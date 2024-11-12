// Attach to the prefab for easier component access by the UI Scripts.
// Otherwise we would need slot.GetChild(0).GetComponentInChildren<Text> etc.
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIItemMallSlot : MonoBehaviour
{
    public UIShowToolTip tooltip;
    public Image image;
    public UIDragAndDropable dragAndDropable;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    public Button unlockButton;
}
