#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

public class FolderCreatorWindow : EditorWindow
{
    private string gameName = "MyGame";
    private static FolderCreatorWindow window;
    private Texture2D infoIcon;

    [MenuItem("MMO-Indie/New Project/Create Default Folder Structure")]
    static void RunOnce()
    {
        // Vérifie si la fenêtre est déjà ouverte pour ne pas en créer une nouvelle
        if (window == null)
        {
            window = GetWindow<FolderCreatorWindow>(true, "Create Folder Project", true);
            window.minSize = new Vector2(400, 200);  // Taille minimale de la fenêtre
            window.maxSize = new Vector2(400, 200);  // Taille maximale pour éviter le redimensionnement
            window.Show();  // Affiche la fenêtre comme une fenêtre indépendante
        }
        else
        {
            window.Focus();  // Si la fenêtre est déjà ouverte, on la met au premier plan
        }
    }

    private void OnEnable()
    {
        // Charger une icône d'information (Unity propose certaines icônes par défaut)
        infoIcon = EditorGUIUtility.IconContent("console.infoicon").image as Texture2D;
    }

    private void OnGUI()
    {
        // Espace pour aérer l'interface
        GUILayout.Space(10);

        // Affiche l'icône et un encadré avec une description (sans l'icône par défaut de HelpBox)
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(infoIcon, GUILayout.Width(32), GUILayout.Height(32));  // Affiche l'icône
        GUILayout.BeginVertical("box");  // Contour encadré
        GUILayout.Label("This tool helps you quickly create a folder structure for organizing your Unity project. You can specify the game name, and the necessary directories will be automatically generated under the 'Assets/uMMORPG' folder.", EditorStyles.wordWrappedLabel);
        GUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(20);

        // Champ pour entrer le nom du jeu
        GUILayout.Label("Game Name", EditorStyles.boldLabel);
        gameName = EditorGUILayout.TextField("Game Name:", gameName);

        // Bouton pour créer la structure des dossiers
        if (GUILayout.Button("Create the folder structure"))
        {
            if (string.IsNullOrEmpty(gameName))
            {
                EditorUtility.DisplayDialog("Error", "Game name cannot be empty.", "OK");
                return;
            }

            CreateFolders(gameName);
        }
    }

    private void CreateFolders(string gameName)
    {
        string rootPath = $"Assets/uMMORPG/_{gameName}";
        string[] folders = new string[]
        {
            $"{rootPath}/Prefabs",
            $"{rootPath}/Materials",
            $"{rootPath}/Models",
            $"{rootPath}/Resources",
            $"{rootPath}/Scenes",
            $"{rootPath}/Shaders",
            $"{rootPath}/Terrains",
            $"{rootPath}/Textures",
            $"{rootPath}/Animations",
            $"{rootPath}/Sounds"
        };

        foreach (var folder in folders)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }

        AssetDatabase.Refresh();
    }
}
#endif