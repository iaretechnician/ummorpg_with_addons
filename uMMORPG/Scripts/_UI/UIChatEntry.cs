using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIChatEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TextMeshProUGUI text;

    // keep all the message info in case it's needed to reply etc.
    [HideInInspector] public ChatMessage message;
    public FontStyles mouseOverStyle = FontStyles.Italic;
    FontStyles defaultStyle;

    private void Start()
    {
         text = GetComponent<TextMeshProUGUI>();
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        // can we reply to this message?
        if (!string.IsNullOrWhiteSpace(message.replyPrefix))
        {
            defaultStyle = text.fontStyle;
            text.fontStyle = mouseOverStyle;
        }
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        text.fontStyle = defaultStyle;
    }

    public void OnPointerClick(PointerEventData data)
    {
        // find the chat component in the parents
        GetComponentInParent<UIChat>().OnEntryClicked(this);
    }
}
