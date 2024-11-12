// Attach to the prefab for easier component access by the UI Scripts.
// Otherwise we would need slot.GetChild(0).GetComponentInChildren<Text> etc.
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UINpcQuestSlot : MonoBehaviour
{
    public TextMeshProUGUI descriptionText;
    public Button actionButton;
}
