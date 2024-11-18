#if UNITY_EDITOR && _iMMOTOOLS
using UnityEngine;
using UnityEditor;


public partial class UINpcTrading : MonoBehaviour
{
    private void OnValidate()
    {
        // Appel� lors de la modification du script ou de l'inspecteur dans l'�diteur
        CheckPrefabAssignment();
    }

    private void CheckPrefabAssignment()
    {
        // V�rifie si le prefab est assign�
        if (slotPrefab == null)
        {
            // Affiche une erreur et arr�te le jeu si le prefab n'est pas assign�
            Debug.LogError("Slot Prefab (Universal) is not assigned! Click here to select the GameObject.", this);
            // Arr�te l'ex�cution si en mode Play
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}


[CustomEditor(typeof(UINpcTrading))]
public partial class UINpcTradingEditor : Editor
{
    private string prefabName = "SlotUniversalNpcTrading"; // Nom du prefab par d�faut, peut �tre modifi� selon ton besoin

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        UINpcTrading uiEquipment = (UINpcTrading)target;

        // V�rifier si le champ slotPrefab est assign�
        bool isAssigned = uiEquipment.slotPrefab != null;

        // D�finir la couleur et le texte du bouton selon l'�tat d'assignation
        GUI.backgroundColor = isAssigned ? Color.green : Color.red;
        string buttonText = isAssigned ? "Slot Prefab (Universal) is assigned!" : "Assign Slot Prefab (Universal)";

        if (GUILayout.Button(buttonText))
        {
            if (isAssigned)
            {
                Debug.Log("Slot Prefab (Universal) is already assigned: " + uiEquipment.slotPrefab.name);
            }
            else
            {
                // Rechercher le prefab par son nom
                string[] guids = AssetDatabase.FindAssets(prefabName + " t:Prefab");
                if (guids.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    GameObject selectedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                    if (selectedPrefab != null)
                    {
                        // V�rifier si le prefab contient un composant UI_UniversalSlot
                        UI_UniversalSlot universalSlot = selectedPrefab.GetComponent<UI_UniversalSlot>();
                        if (universalSlot != null)
                        {
                            // Assigner le prefab si le composant est correct
                            uiEquipment.slotPrefab = universalSlot;
                            EditorUtility.SetDirty(uiEquipment);
                            Debug.Log("Prefab assigned: " + selectedPrefab.name);
                        }
                        else
                        {
                            Debug.LogWarning("The selected prefab does not have a UI_UniversalSlot component.");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Prefab not found with the specified name: " + prefabName);
                    }
                }
                else
                {
                    Debug.LogWarning("No prefab found with the specified name: " + prefabName);
                }
            }
        }

        // R�initialiser la couleur
        GUI.backgroundColor = Color.white;
    }
}
#endif
