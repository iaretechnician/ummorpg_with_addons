
#if _iMMOUMACHARACTERS
using UnityEngine;
using UnityEngine.UI;

public class UmaRaceButton : MonoBehaviour
{
    public UmaRaceButtonValue race;

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
        if (creator.dca == null) return;
        string raceName = "";
        if(race.ToString() == "Elf")
        {
            raceName = race.ToString() + " ";
        }
        else
        {
            raceName = race.ToString();
        }
        creator.SwitchRace(raceName + "Male");
    }
}

public enum UmaRaceButtonValue
{
    Human,
    Elf
}

#endif