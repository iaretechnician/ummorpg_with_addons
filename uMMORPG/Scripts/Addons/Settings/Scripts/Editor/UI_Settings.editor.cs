#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UI_Settings))]
public partial class UI_SettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        UI_Settings uiSettings = (UI_Settings)target;

        // Vérifier si tous les champs sont assignés
        bool allAssigned = uiSettings.uiParty != null &&
                           uiSettings.uiCharacterInfo != null &&
#if !_iMMOCOMPLETECHAT
                           uiSettings.uiChat != null &&
#endif
                           uiSettings.uiCrafting != null &&
                           uiSettings.uiEquipment != null &&
                           uiSettings.uiGuild != null &&
                           uiSettings.uiInventory != null &&
                           uiSettings.uiItemMall != null &&
                           uiSettings.uiQuests != null &&
                           uiSettings.uiSkills != null
#if _iMMOSKILLCATEGORY
                           && uiSettings.ui_Skills != null
#endif
                           ;

        // Changer la couleur du bouton en fonction de l'état d'assignation
        GUI.backgroundColor = allAssigned ? Color.green : Color.red;
        
        if (GUILayout.Button("Auto Assign GameObject"))
        {
            AssignGameObjectsAutomatically(uiSettings);
        }

        // Réinitialiser la couleur
        GUI.backgroundColor = Color.white;
    }

    private void AssignGameObjectsAutomatically(UI_Settings uiSettings)
    {
        // Rechercher tous les GameObjects dans la scène avec les composants spécifiés
        UIParty[] uiParty = FindObjectsByType<UIParty>(FindObjectsSortMode.None);
        UICharacterInfo[] uICharacterInfo = FindObjectsByType<UICharacterInfo>(FindObjectsSortMode.None);
#if !_iMMOCOMPLETECHAT
        UIChat[] uIChat = FindObjectsOfType<UIChat>();
#endif
        UICrafting[] uICrafting = FindObjectsByType<UICrafting>(FindObjectsSortMode.None);
        UIEquipment[] uIEquipment = FindObjectsByType<UIEquipment>(FindObjectsSortMode.None);
        UIGuild[] uIGuild = FindObjectsByType<UIGuild>(FindObjectsSortMode.None);
        UIInventory[] uIInventory = FindObjectsByType<UIInventory>(FindObjectsSortMode.None);
        UIItemMall[] uIItemMall = FindObjectsByType<UIItemMall>(FindObjectsSortMode.None);
        UIQuests[] uIQuests = FindObjectsByType<UIQuests>(FindObjectsSortMode.None);
        UISkills[] uISkills = FindObjectsByType<UISkills>(FindObjectsSortMode.None);
#if _iMMOSKILLCATEGORY
        UI_Skills[] uI_Skills = FindObjectsByType<UI_Skills>(FindObjectsSortMode.None);
#endif
        // Assigner les GameObjects trouvés
        uiSettings.uiParty = uiParty.Length > 0 ? uiParty[0] : null;
        uiSettings.uiCharacterInfo = uICharacterInfo.Length > 0 ? uICharacterInfo[0] : null;
#if !_iMMOCOMPLETECHAT
        uiSettings.uiChat = uIChat.Length > 0 ? uIChat[0] : null;
#endif
        uiSettings.uiCrafting = uICrafting.Length > 0 ? uICrafting[0] : null;
        uiSettings.uiEquipment = uIEquipment.Length > 0 ? uIEquipment[0] : null;
        uiSettings.uiGuild = uIGuild.Length > 0 ? uIGuild[0] : null;
        uiSettings.uiInventory = uIInventory.Length > 0 ? uIInventory[0] : null;
        uiSettings.uiItemMall = uIItemMall.Length > 0 ? uIItemMall[0] : null;
        uiSettings.uiQuests = uIQuests.Length > 0 ? uIQuests[0] : null;
        uiSettings.uiSkills = uISkills.Length > 0 ? uISkills[0] : null;
#if _iMMOSKILLCATEGORY
        uiSettings.ui_Skills = uI_Skills.Length > 0 ? uI_Skills[0] : null;
#endif

        // Marquer l'objet comme modifié pour sauvegarder les changements
        EditorUtility.SetDirty(uiSettings);
    }
}
#endif