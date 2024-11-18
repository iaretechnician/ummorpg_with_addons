#if _iMMOUMACHARACTERS

using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UmaChangeClothingButton : MonoBehaviour
{
    [Header("Decrease Clothing = false")]
    public bool increase = true;
    public TextMeshProUGUI indexText;
    private UI_CharacterCreation creator;
    private Button changeButton;

    private void Start()
    {
        //creator = FindObjectOfType<UI_CharacterCreation>();
        creator = FindFirstObjectByType<UI_CharacterCreation>();
        changeButton = gameObject.GetComponent<Button>();
        changeButton.onClick.SetListener(ChangeClothing);
    }

    private int index = 0;

    public void ChangeClothing()
    {
        if (creator.dca == null) return;

        bool male = creator.dca.activeRace.name == creator.currentPlayer.playerAddonsConfigurator.tmpl_UMACharacterCreation.dcaRaceMale ? true : false;

        if (male) // Male
        {
            if (increase) // Increase
                if (creator.maleClothingIndex >= creator.currentPlayer.playerAddonsConfigurator.tmpl_UMACharacterCreation.clothingsMale.Length - 1)
                {
                    creator.maleClothingIndex = 0;
                    index = creator.maleClothingIndex;
                }
                else { creator.maleClothingIndex += 1; index = creator.maleClothingIndex; }
            if (!increase) // Decrease
                if (creator.maleClothingIndex == 0)
                {
                    creator.maleClothingIndex = creator.currentPlayer.playerAddonsConfigurator.tmpl_UMACharacterCreation.clothingsMale.Length - 1;
                    index = creator.maleClothingIndex;
                }
                else { creator.maleClothingIndex -= 1; index = creator.maleClothingIndex; }
        }
        if (!male) // Female
        {
            if (increase) // Increase
                if (creator.femaleClothingIndex >= creator.currentPlayer.playerAddonsConfigurator.tmpl_UMACharacterCreation.clothingsFemale.Length - 1)
                {
                    creator.femaleClothingIndex = 0;
                    index = creator.femaleClothingIndex;
                }
                else { creator.femaleClothingIndex += 1; index = creator.femaleClothingIndex; }
            if (!increase) // Decrease
                if (creator.femaleClothingIndex == 0)
                {
                    creator.femaleClothingIndex = creator.currentPlayer.playerAddonsConfigurator.tmpl_UMACharacterCreation.clothingsFemale.Length - 1;
                    index = creator.femaleClothingIndex;
                }
                else { index = creator.femaleClothingIndex; index = creator.femaleClothingIndex; }
        }
        creator.SelectClothing(index);

        if (indexText != null)
            indexText.text = (index + 1).ToString();
    }
}

#endif