using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace ToolsFilesPatcher.Window
{
    public class ScriptUpdateEditorWindow : EditorWindow
    {
        // URL de l'API
        private const string API_URL = "https://api.mmo-indie.com/Tools_Updater/";

        // Version du projet
        private string directoryPath = "Assets/uMMORPG/Scripts/Addons/_Tools/Editor/ToolsFilesPatcher/Core";
        // Dictionnaire pour stocker les scripts locaux
        private ScriptInfoList localScripts = new() { script = new List<ScriptInfo>() };
        private ScriptInfoList onlineScripts = new() { script = new List<ScriptInfo>() };
        private List<ScriptInfo> modifiedScripts = new();
        private List<ScriptInfo> downloadScripts = new();
        private List<ScriptInfo> removeScripts = new();
        // Fonction pour afficher la fenêtre de l'éditeur
        //[MenuItem("Tools/Script Updater")]
        public static void ShowWindow()
        {
            GetWindow<ScriptUpdateEditorWindow>();

        }

        private void OnEnable()
        {
            localScripts.script.Clear();
            onlineScripts.script.Clear();
            modifiedScripts.Clear();
            downloadScripts.Clear();
            removeScripts.Clear();
            GetFiles(directoryPath, localScripts);
            CheckForUpdatesAsync();

        }

        // Fonction pour afficher l'interface de la fenêtre
        private void OnGUI()
        {
            // Titre
            GUILayout.Label("Script Updater", EditorStyles.boldLabel);

            // Bouton pour vérifier les mises à jour
            if (GUILayout.Button("Search updated script"))
            {
                localScripts.script.Clear();
                GetFiles(directoryPath, localScripts);
                CheckForUpdatesAsync();
            }
            var labelTitle = new GUIStyle(GUI.skin.label);
            labelTitle.richText = true;
            labelTitle.fontSize = 18;
            // Liste des scripts modifiés
            if (modifiedScripts != null && modifiedScripts.Count > 0)
            {
                GUILayout.Label("<b><color=Orange>Modified Scripts :</color></b> ", labelTitle);
                foreach (ScriptInfo scriptInfo in modifiedScripts)
                {
                    GUILayout.Label("Name: " + scriptInfo.name + " | Local MD5: " + scriptInfo.md5 + " | Online MD5: " + scriptInfo.md5Online);
                }
            }
            // Liste des scripts modifiés
            if (downloadScripts != null && downloadScripts.Count > 0)
            {
                GUILayout.Label("<b><color=red>Download Scripts :</color></b> ", labelTitle);
                foreach (ScriptInfo scriptInfo in downloadScripts)
                {
                    GUILayout.Label(scriptInfo.name);
                }
            }
            // Liste des scripts modifiés
            if (removeScripts != null && removeScripts.Count > 0)
            {
                GUILayout.Label("<b><color=red>Remove Scripts :</color></b> ", labelTitle);
                foreach (ScriptInfo scriptInfo in removeScripts)
                {
                    GUILayout.Label(scriptInfo.name);
                }
            }

            // Bouton pour vérifier les mises à jour
            if (removeScripts.Count > 0 || downloadScripts.Count > 0 || modifiedScripts.Count > 0)
            {
                if (GUILayout.Button("Update all script"))
                {
                    ScriptUpdatesAsync();
                }
            }
        }
        private async void ScriptUpdatesAsync()
        {
            if (modifiedScripts.Count > 0)
            {
                List<ScriptInfo> scriptsModified = new List<ScriptInfo>(modifiedScripts);
                foreach (ScriptInfo scriptInfo in scriptsModified)
                {
                    await DownloadUpdateAsync(scriptInfo.path);
                    modifiedScripts.Remove(scriptInfo);
                }
            }
            if (downloadScripts.Count > 0)
            {
                List<ScriptInfo> scriptsDownload = new List<ScriptInfo>(downloadScripts);
                foreach (ScriptInfo scriptInfo in scriptsDownload)
                {
                    await DownloadUpdateAsync(scriptInfo.path);
                    downloadScripts.Remove(scriptInfo);
                }
            }
            if (removeScripts.Count > 0)
            {
                List<ScriptInfo> scriptsToRemove = new List<ScriptInfo>(removeScripts);
                foreach (ScriptInfo scriptInfo in scriptsToRemove)
                {
                    await DeleteLocalScriptAsync(scriptInfo.path);
                    removeScripts.Remove(scriptInfo);
                }
            }
            AssetDatabase.Refresh();
        }

        private async Task DownloadUpdateAsync(string pathFile)
        {
            string fullPath = Path.Combine(directoryPath, pathFile);
            string directory = Path.GetDirectoryName(fullPath);

            EnsureDirectoryExists(directory);

            using (UnityWebRequest www = UnityWebRequest.Get(API_URL + "/Core/" + pathFile))
            {
                var asyncOperation = www.SendWebRequest();

                while (!asyncOperation.isDone)
                {
                    await Task.Delay(100); // Attendez un court instant avant de vérifier à nouveau
                }

                if (www.result == UnityWebRequest.Result.Success)
                {
                    // Le téléchargement est terminé avec succès
                    byte[] data = www.downloadHandler.data;

                    // Sauvegarde du fichier localement
                    File.WriteAllBytes(fullPath, data);
                }
                else
                {
                    // Une erreur s'est produite lors du téléchargement
                    Debug.LogError("Erreur de téléchargement : " + www.error);
                }
            }
        }

        private void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private async Task DeleteLocalScriptAsync(string filePath)
        {
            await Task.Run(() =>
            {
                try
                {
                    // Vérifie si le fichier existe avant de tenter de le supprimer
                    if (File.Exists(filePath))
                    {
                        // Supprime le fichier
                        File.Delete(filePath);
                        File.Delete(filePath + ".meta");
                    }
                    else
                    {
                        Debug.LogWarning("Le fichier n'existe pas : " + filePath);
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("Erreur lors de la suppression du fichier : " + ex.Message);
                }
            });
        }

        // Fonction pour démarrer la vérification des mises à jour
        private async void CheckForUpdatesAsync()
        {
            modifiedScripts.Clear();
            downloadScripts.Clear();
            removeScripts.Clear();
            modifiedScripts = new List<ScriptInfo>();
            // Lancer la vérification des mises à jour en arrière-plan
            await GetModifiedScriptsAsync();

            // Liste des scripts modifiés
            Debug.Log("Online script count :" + onlineScripts.script.Count);
            Debug.Log("Local script count :" + localScripts.script.Count);

            // Mettre à jour l'interface utilisateur après la fin de la tâche
            await GetVerifyModifiedScriptsAsync();
        }

        private async Task GetVerifyModifiedScriptsAsync()
        {
            await Task.Run(() =>
            {
                // Chercher les fichiers obsolètes locaux et les retirer

                foreach (ScriptInfo localScript in localScripts.script.ToList())
                {
                    ScriptInfo correspondingOnlineScript = onlineScripts.script.FirstOrDefault(s => s.name == localScript.name);
                    if (correspondingOnlineScript.name != null && localScript.md5 != correspondingOnlineScript.md5)
                    {
                        // Mettre à jour le MD5 en ligne dans le script local
                        correspondingOnlineScript.md5Online = correspondingOnlineScript.md5;
                        correspondingOnlineScript.md5 = localScript.md5;
                        // Ajouter à la liste des scripts modifiés
                        modifiedScripts.Add(correspondingOnlineScript);
                    }
                }

                // Chercher les nouveaux scripts en ligne et les télécharger
                foreach (ScriptInfo onlineScript in onlineScripts.script)
                {
                    if (!localScripts.script.Any(s => s.name == onlineScript.name))
                    {
                        // Télécharger le script
                        downloadScripts.Add(onlineScript);
                    }
                }

                // Chercher les scripts locaux non présents en ligne pour les marquer à la suppression
                foreach (ScriptInfo localScript in localScripts.script)
                {
                    if (!onlineScripts.script.Any(s => s.name == localScript.name))
                    {
                        // Ajouter à la liste des scripts à supprimer
                        removeScripts.Add(localScript);
                    }
                }
            });
        }

        private Task GetModifiedScriptsAsync()
        {
            // Créer un TaskCompletionSource pour surveiller la fin de l'opération asynchrone
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

            UnityWebRequest request = UnityWebRequest.Get(API_URL);
            request.timeout = 15;
            request.method = UnityWebRequest.kHttpVerbGET;

            UnityWebRequestAsyncOperation asyncOperation = request.SendWebRequest();

            // Catch the event as it's own method
            asyncOperation.completed += (obj) =>
            {
                onlineScripts = JsonUtility.FromJson<ScriptInfoList>(request.downloadHandler.text);

                // Marquer la tâche comme terminée
                tcs.SetResult(null);
            };

            // Retourner la tâche associée à la fin de l'opération asynchrone
            return tcs.Task;
        }

        // Récupération de la liste des fichiers à modifier en cache (locals)
        private void GetFiles(string directoryPath, ScriptInfoList files)
        {
            // Parcourir les fichiers du dossier actuel
            foreach (string file in Directory.GetFiles(directoryPath))
            {
                if (Path.GetExtension(file) == ".csharp")
                {
                    string relativePath = Path.GetRelativePath(directoryPath, file);
                    ScriptInfo script = new()
                    {
                        name = relativePath,
                        path = file,
                        md5 = GetMD5(file)
                    };
                    localScripts.script.Add(script);
                }
            }

            // Parcourir les sous-dossiers du dossier actuel
            foreach (string subdirectory in Directory.GetDirectories(directoryPath))
            {
                GetFiles(subdirectory, files);
            }
        }

        // Récupréation du MD5 d'un fichier local
        public static string GetMD5(string filePath)
        {
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(filePath);
            return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty);
        }

        // Structure pour stocker les informations d'un script
        [Serializable]
        public struct ScriptInfo
        {
            public string name;
            public string path;
            public string md5;
            public string md5Online; // Nouveau champ pour le MD5 en ligne
        }

        [Serializable]
        public class ScriptInfoList
        {
            public List<ScriptInfo> script;
        }
    }
}