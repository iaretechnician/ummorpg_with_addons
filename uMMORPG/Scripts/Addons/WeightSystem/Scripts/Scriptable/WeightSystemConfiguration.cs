using UnityEngine;

[CreateAssetMenu(fileName = "Weight System Configuration", menuName = "ADDON/Templates/WeightSystem/Weight System Configuration", order = 999)]
public class WeightSystemConfiguration : ScriptableObject
{
    public TargetBuffSkill burdenedBuff;
    public int maxWeight;
    public int carryPerPoint;
#if _iMMOATTRIBUTES
    public AttributeTemplate weightAttribute;
#endif
    public int maxBurdenLevel;
    public bool equipmentNotIncluded;

}