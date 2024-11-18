#if UNITY_EDITOR && _iMMOTOOLS
using UnityEngine;
using UnityEditor;


//[ExecuteInEditMode]
public partial class UICrafting : MonoBehaviour
{
    private void OnValidate()
    {
        // Appelé lors de la modification du script ou de l'inspecteur dans l'éditeur
        CheckPrefabAssignment();
    }

    private void CheckPrefabAssignment()
    {
        // Vérifie si le prefab est assigné
        if (ingredientSlotPrefab == null)
        {
            // Affiche une erreur et arrête le jeu si le prefab n'est pas assigné
            Debug.LogError("Slot Prefab (Universal) is not assigned! Click here to select the GameObject.", this);
            // Arrête l'exécution si en mode Play
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}


[CustomEditor(typeof(UICrafting))]
public partial class UICraftingEditor : Editor
{
    private string prefabName = "SlotUniversalCraftingIngredient"; // Nom du prefab par défaut, peut être modifié selon ton besoin

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        UICrafting uiEquipment = (UICrafting)target;

        // Vérifier si le champ slotPrefab est assigné
        bool isAssigned = uiEquipment.ingredientSlotPrefab != null;

        // Définir la couleur et le texte du bouton selon l'état d'assignation
        GUI.backgroundColor = isAssigned ? Color.green : Color.red;
        string buttonText = isAssigned ? "Slot Prefab (Universal) is assigned!" : "Assign Slot Prefab (Universal)";

        if (GUILayout.Button(buttonText))
        {
            if (isAssigned)
            {
                Debug.Log("Slot Prefab (Universal) is already assigned: " + uiEquipment.ingredientSlotPrefab.name);
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
                        // Vérifier si le prefab contient un composant UI_UniversalSlot
                        UI_UniversalSlot universalSlot = selectedPrefab.GetComponent<UI_UniversalSlot>();
                        if (universalSlot != null)
                        {
                            // Assigner le prefab si le composant est correct
                            uiEquipment.ingredientSlotPrefab = universalSlot;
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

        // Réinitialiser la couleur
        GUI.backgroundColor = Color.white;
    }
}
#endif
