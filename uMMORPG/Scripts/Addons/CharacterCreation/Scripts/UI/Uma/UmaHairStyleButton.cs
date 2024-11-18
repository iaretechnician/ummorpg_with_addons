#if _iMMOUMACHARACTERS

using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UmaHairStyleButton : MonoBehaviour
{
    public int MaleHairStyleIndex = 0;
    public int FemaleHairStyleIndex = 0;
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

    public void ChangeHair()
    {
        if (creator.dca == null) return;

        bool male = creator.dca.activeRace.name == creator.currentPlayer.playerAddonsConfigurator.tmpl_UMACharacterCreation.dcaRaceMale ? true : false;
        int hairid = male ? MaleHairStyleIndex : FemaleHairStyleIndex;
        if (male && creator.currentPlayer.playerAddonsConfigurator.tmpl_UMACharacterCreation.maleHairStyles.Count > 0 && hairid <= creator.currentPlayer.playerAddonsConfigurator.tmpl_UMACharacterCreation.maleHairStyles.Count - 1)
        {

        }
        else if(!male && creator.currentPlayer.playerAddonsConfigurator.tmpl_UMACharacterCreation.femaleHairStyles.Count > 0 && hairid <= creator.currentPlayer.playerAddonsConfigurator.tmpl_UMACharacterCreation.femaleHairStyles.Count - 1){

        }
        else
            hairid = 0;

        
        creator.SelectHair(hairid);

        if (indexText != null)
            indexText.text = ((male ? MaleHairStyleIndex : FemaleHairStyleIndex) + 1).ToString();
    }
}

#endif