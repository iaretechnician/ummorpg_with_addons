using UnityEngine;
using TMPro;
using UnityEngine.UI;
#if _iMMOUMACHARACTERS
using UMA;
#endif

public class UmaSelectClothingButton : MonoBehaviour
{
    [Header("Next / Prev skin")]
    private UI_CharacterCreation creator;
    public Button btnIincrease;
    public Button btnDecrease;
    private void Start()
    {
        //creator = FindObjectOfType<UI_CharacterCreation>();
        creator = FindFirstObjectByType<UI_CharacterCreation>();
        btnIincrease.onClick.SetListener(ChangeClothinIncrease);
        btnDecrease.onClick.SetListener(ChangeClothinDecrease);

    }
#if _iMMOUMACHARACTERS
    public int index = 0;
#endif


    public void ChangeClothinIncrease()
    {
        ChangeClothing();
    }

    public void ChangeClothinDecrease()
    {
        ChangeClothing(false);
    }

    public void ChangeClothing(bool increase = true)
    {
#if _iMMOUMACHARACTERS
        if (creator.dca == null) return;

        bool male = creator.dca.activeRace.name == creator.currentPlayer.playerAddonsConfigurator.tmpl_UMACharacterCreation.dcaRaceMale;
        int max = (male) ? creator.currentPlayer.playerAddonsConfigurator.tmpl_UMACharacterCreation.clothingsMale.Length : creator.currentPlayer.playerAddonsConfigurator.tmpl_UMACharacterCreation.clothingsFemale.Length;

        // Increase
        if (increase)
        {
            index = (index >= max - 1) ? 0 : (index + 1);
        }
        // Decrease
        else
        {
            index = (index == 0 ) ? (index = max -1) : (index - 1);
        }

        if(index > -1)
            creator.SelectClothing(index);

#endif
    }
}