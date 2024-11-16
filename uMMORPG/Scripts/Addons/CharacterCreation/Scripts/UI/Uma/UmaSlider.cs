using UnityEngine;
using UnityEngine.UI;

public class UmaSlider : MonoBehaviour
{
    public float min = 0.4f, max = 0.6f;

    public UmaSliderTypes type;

    private UI_CharacterCreation creator;
    private Slider changeSlider;

    private void Start()
    {
        //creator = FindObjectOfType<UI_CharacterCreation>();
        creator = FindFirstObjectByType<UI_CharacterCreation>();
        changeSlider = gameObject.GetComponent<Slider>();
        changeSlider.minValue = min;
        changeSlider.maxValue = max;
        changeSlider.value = (min + max) / 2;
        changeSlider.onValueChanged.SetListener(PerformTask);
    }

    // Start is called before the first frame update
    public void PerformTask(float value)
    {
#if _iMMOUMACHARACTERS
        if (creator.dca == null) return;
        if (value > creator.dca.umaData.GetAllDna().Length) return;

        creator.dca.umaData.GetAllDna()[0].SetValue(type.GetHashCode() + (creator.dca.activeRace.name == creator.currentPlayer.playerAddonsConfigurator.tmpl_UMACharacterCreation.dcaRaceMale ? 3 : 0), value);
        creator.dca.UpdateUMA();
#endif
    }
}

public enum UmaSliderTypes
{
    Height,
    Head_Size,
    Head_Width,
    Neck_Thickness,
    Arm_Length,
    Forearm_Length,
    Arm_Width,
    Forearm_Widgth,
    Hand_Size,
    Feet_Size,
    Leg_Seperation,
    Upper_Muscle,
    Lower_Muscle,
    Upper_Weight,
    Lower_Weight,
    Leg_Size,
    Belly_Size,
    Waist,
    Gluteus_Size,
    Ear_Size,
    Ear_Position,
    Ear_Rotation,
    Nose_Size,
    Nose_Curve,
    Nose_Width,
    Nose_Inclination,
    Nose_Flatten,
    Chin_Size,
    Chin_Pronounced,
    Chin_Position,
    Mandible_Size,
    Jaw_Size,
    Jaw_Position,
    Cheek_Size,
    Cheek_Position,
    Low_Cheek_Pronounced,
    Low_Cheeck_Position,
    Forehead_Size,
    Forehead_Position,
    Lip_Size,
    Mouth_Size,
    Eye_Rotation,
    Eye_Size,
    Breast_Size
}
