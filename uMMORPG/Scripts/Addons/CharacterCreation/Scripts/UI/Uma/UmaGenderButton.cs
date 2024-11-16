
using UnityEngine;
using UnityEngine.UI;

public class UmaGenderButton : MonoBehaviour
{
    public UmaGenderButtonValue gender;

    private UI_CharacterCreation creator;
    private Button changeButton;

    private void Start()
    {
        //creator = FindObjectOfType<UI_CharacterCreation>();
        creator = FindFirstObjectByType<UI_CharacterCreation>();
        changeButton = gameObject.GetComponent<Button>();
        changeButton.onClick.SetListener(PerformTask);
    }

    // Start is called before the first frame update
    public void PerformTask()
    {
#if _iMMOUMACHARACTERS
        if (creator.dca == null) return;
        creator.SwitchGender((gender == UmaGenderButtonValue.Male) ? creator.currentPlayer.playerAddonsConfigurator.tmpl_UMACharacterCreation.dcaRaceMale : creator.currentPlayer.playerAddonsConfigurator.tmpl_UMACharacterCreation.dcaRaceFemale);
#endif
    }
}

public enum UmaGenderButtonValue
{
    Male,
    Female
}

