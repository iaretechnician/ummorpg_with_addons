using UnityEngine;
using TMPro;
using UnityEngine.UI;
#if _iMMOUMACHARACTERS
using UMA;
#endif

public class UmaHairStylesButton : MonoBehaviour
{
    [Header("Decrease Hairstyle = false")]
    public bool increase = true;
    public TextMeshProUGUI indexText;
    private UI_CharacterCreation creator;
    private Button changeButton;

    private void Start()
    {
        //creator = FindObjectOfType<UI_CharacterCreation>();
        creator = FindFirstObjectByType<UI_CharacterCreation>();
        changeButton = gameObject.GetComponent<Button>();
        changeButton.onClick.SetListener(ChangeHair);
    }
#if _iMMOUMACHARACTERS
    private int index = 0;
#endif

    public void ChangeHair()
    {
#if _iMMOUMACHARACTERS
        if (creator.dca == null) return;

        bool male = creator.dca.activeRace.name == creator.currentPlayer.playerAddonsConfigurator.tmpl_UMACharacterCreation.dcaRaceMale ? true : false;

        if (male) // Male
        {
            if (increase) // Increase
                if (creator.maleIndex >= creator.currentPlayer.playerAddonsConfigurator.tmpl_UMACharacterCreation.maleHairStyles.Count - 1)
                {
                    creator.maleIndex = 0;
                    index = creator.maleIndex;
                }
                else { creator.maleIndex += 1; index = creator.maleIndex; }
            if (!increase) // Decrease
                if (creator.maleIndex == 0)
                {
                    creator.maleIndex = creator.currentPlayer.playerAddonsConfigurator.tmpl_UMACharacterCreation.maleHairStyles.Count - 1;
                    index = creator.maleIndex;
                }
                else { creator.maleIndex -= 1; index = creator.maleIndex; }
        }
        if (!male) // Female
        {
            if (increase) // Increase
                if (creator.femaleIndex >= creator.currentPlayer.playerAddonsConfigurator.tmpl_UMACharacterCreation.femaleHairStyles.Count - 1)
                {
                    creator.femaleIndex = 0;
                    index = creator.femaleIndex;
                }
                else { creator.femaleIndex += 1; index = creator.femaleIndex; }
            if (!increase) // Decrease
                if (creator.femaleIndex == 0)
                {
                    creator.femaleIndex = creator.currentPlayer.playerAddonsConfigurator.tmpl_UMACharacterCreation.femaleHairStyles.Count - 1;
                    index = creator.femaleIndex;
                }
                else { creator.femaleIndex -= 1; index = creator.femaleIndex; }
        }
        creator.SelectHair(index);

        if (indexText != null)
            indexText.text = (index + 1).ToString();
#endif
    }
}
