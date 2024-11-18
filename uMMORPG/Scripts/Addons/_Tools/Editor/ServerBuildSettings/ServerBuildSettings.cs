#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class ServerBuildSettings : IActiveBuildTargetChanged
{
    public int callbackOrder => 0;

    // Liste pour sauvegarder les terrains désactivés
    private static List<Terrain> savedTerrains = new List<Terrain>();

    // Lorsque la plateforme de build change, ce callback est appelé
    public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
    {
        CheckServerBuildMode();
    }

    // Méthode qui vérifie si le mode "Server" est activé dans les Build Settings
    public static void CheckServerBuildMode()
    {
        if (EditorUserBuildSettings.standaloneBuildSubtarget == StandaloneBuildSubtarget.Server)
        {
            DisableTerrainsInAllScenes();
        }
        else
        {
            RestoreTerrainsInAllScenes();
        }
    }

    // Désactive tous les terrains dans chaque scène active du Build Settings
    private static void DisableTerrainsInAllScenes()
    {
        foreach (var scenePath in GetActiveScenePaths())
        {
            // Charge la scène en mode Additive sans la rendre active
            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);

            // Désactive les terrains dans cette scène
            Terrain[] terrainsInScene = scene.GetRootGameObjects()
                .SelectMany(go => go.GetComponentsInChildren<Terrain>(true))
                .ToArray();

            foreach (Terrain terrain in terrainsInScene)
            {
                if (terrain != null)
                {
                    terrain.enabled = false;
                    savedTerrains.Add(terrain);
                }
            }

            // Sauvegarde la scène pour que les changements soient pris en compte lors du build
            EditorSceneManager.SaveScene(scene);
            Debug.Log($"Terrains désactivés et scène sauvegardée : {scenePath}");

            // Décharge la scène pour éviter de la garder en mémoire
            EditorSceneManager.CloseScene(scene, true);
        }
    }

    // Réactive tous les terrains dans chaque scène active du Build Settings
    private static void RestoreTerrainsInAllScenes()
    {
        foreach (var scenePath in GetActiveScenePaths())
        {
            // Charge la scène en mode Additive sans la rendre active
            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);

            // Réactive les terrains dans cette scène
            Terrain[] terrainsInScene = scene.GetRootGameObjects()
                .SelectMany(go => go.GetComponentsInChildren<Terrain>(true))
                .ToArray();

            foreach (Terrain terrain in terrainsInScene)
            {
                if (terrain != null)
                {
                    terrain.enabled = true;
                }
            }

            // Sauvegarde la scène pour que les changements soient pris en compte
            EditorSceneManager.SaveScene(scene);
            Debug.Log($"Terrains réactivés et scène sauvegardée : {scenePath}");

            // Décharge la scène pour éviter de la garder en mémoire
            EditorSceneManager.CloseScene(scene, true);
        }

        savedTerrains.Clear();
    }

    // Méthode pour obtenir tous les chemins des scènes actives dans le Build Settings
    private static List<string> GetActiveScenePaths()
    {
        return EditorBuildSettings.scenes
            .Where(scene => scene.enabled)  // On ne prend que les scènes actives dans le Build Settings
            .Select(scene => scene.path)    // On récupère le chemin des scènes
            .ToList();
    }
}
#endif
