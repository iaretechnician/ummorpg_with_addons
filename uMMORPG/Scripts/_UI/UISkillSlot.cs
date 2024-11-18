// Attach to the prefab for easier component access by the UI Scripts.
// Otherwise we would need slot.GetChild(0).GetComponentInChildren<Text> etc.
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISkillSlot : MonoBehaviour
{
    public UIShowToolTip tooltip;
    public UIDragAndDropable dragAndDropable;
    public Image image;
    public Button button;
    public GameObject cooldownOverlay;
    public TextMeshProUGUI cooldownText;
    public Image cooldownCircle;
    public TextMeshProUGUI descriptionText;
    public Button upgradeButton;
}
