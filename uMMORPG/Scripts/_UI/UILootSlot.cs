// Attach to the prefab for easier component access by the UI Scripts.
// Otherwise we would need slot.GetChild(0).GetComponentInChildren<Text> etc.
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILootSlot : MonoBehaviour
{
    public Button button;
    public UIShowToolTip tooltip;
    public UIDragAndDropable dragAndDropable;
    public Image image;
    public TextMeshProUGUI nameText;
    public GameObject amountOverlay;
    public TextMeshProUGUI amountText;
}
