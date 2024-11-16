
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UmaEditColors : MonoBehaviour, IPointerClickHandler
{
    public Color selectedColor;
    public UmaColorTypes Category;
    private UI_CharacterCreation creator;
    public Image imageColor;

    private void OnEnable()
    {
        //creator = FindObjectOfType<UI_CharacterCreation>();
        creator = FindFirstObjectByType<UI_CharacterCreation>();
#if _iMMOUMACHARACTERS
        if(Category == UmaColorTypes.Skin)
            imageColor.sprite = creator.currentPlayer.playerAddonsConfigurator.tmpl_UMACharacterCreation.skinColor;
#endif
    }

    public void OnPointerClick(PointerEventData eventData)
    {
#if _iMMOUMACHARACTERS

        if (creator.dca == null) return;

        selectedColor = GetColor(GetPointerUVPosition());

        creator.ChangeColor(Category, selectedColor);
#endif
    }

    private Color GetColor(Vector2 pos)
    {
        Texture2D texture = GetComponent<Image>().sprite.texture;
        Color selected = texture.GetPixelBilinear(pos.x, pos.y);
        selected.a = 1; // force full alpha
        return selected;
    }

    private Vector2 GetPointerUVPosition()
    {
        Vector3[] imageCorners = new Vector3[4];
        gameObject.GetComponent<RectTransform>().GetWorldCorners(imageCorners);
        float texWidth = imageCorners[2].x - imageCorners[0].x;
        float texHeight = imageCorners[2].y - imageCorners[0].y;
        float uvX = (Input.mousePosition.x - imageCorners[0].x) / texWidth;
        float uvY = (Input.mousePosition.y - imageCorners[0].y) / texHeight;
        return new Vector2(uvX, uvY);
    }
}

public enum UmaColorTypes
{
    Skin,
    Eyes,
    Hair,
    Undies
}
