namespace FrogGame.Editor
{
    using System.IO;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public static class SceneCreatorTool
    {
        private const string MENU_NAME = "Artist Tools";
        private const string TARGET_FOLDER = "Assets/Scenes/Artist Scenes";

        [MenuItem("Frog Tools/" + MENU_NAME + "/Create/New Scene")]
        public static void CreateNewScene() => ScriptableWizard.DisplayWizard<CreateSceneWizard>("Create New Scene", "Create Scene");

        private class CreateSceneWizard : ScriptableWizard
        {
            [Header("Scene Setup")]
            [Tooltip("Scene file name")]
            public string SceneName = "GameplayTest_Artist";

            [Header("Required Assets")]
            public GameObject CameraManagerPrefab;
            public GameObject GridPrefab;
            public GameObject PlayerPrefab;

            private void OnEnable()
            {
                if (CameraManagerPrefab == null)
                {
                    CameraManagerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Camera Manager.prefab");
                }

                if (GridPrefab == null)
                {
                    GridPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Tilemap/Grid.prefab");
                }

                if (PlayerPrefab == null)
                {
                    PlayerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Player.prefab");
                }
            }

            private void OnWizardCreate()
            {
                if (string.IsNullOrWhiteSpace(SceneName))
                {
                    Debug.LogError("[Frog Tools] The scene name cannot be empty.");

                    return;
                }

                if (!ValidatePrefabs())
                {
                    return;
                }

                if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    Debug.LogWarning("[Frog Tools] Scene creation cancelled by user.");

                    return;
                }

                EnsureDirectoryExists(TARGET_FOLDER);

                string fullScenePath = Path.Combine(TARGET_FOLDER, $"{SceneName}.unity").Replace("\\", "/");

                if (File.Exists(fullScenePath))
                {
                    bool overwrite = EditorUtility.DisplayDialog(
                        "The scene already exists",
                        $"There is already a scene called '{SceneName}' at '{TARGET_FOLDER}'. Do you want to overwrite it?",
                        "Yes, overwrite",
                        "Cancel"
                    );

                    if (!overwrite) return;
                }

                Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

                InstantiatePrefab(CameraManagerPrefab, "Camera Manager");
                InstantiatePrefab(GridPrefab, "Grid");
                InstantiatePrefab(PlayerPrefab, "Player");

                bool saveSuccess = EditorSceneManager.SaveScene(newScene, fullScenePath);

                if (saveSuccess)
                {
                    Debug.Log($"[Frog Tools] Scene '{SceneName}' successfully saved at: {fullScenePath}");
                }
                else
                {
                    Debug.LogError($"[Frog Tools] Failed to save scene at {fullScenePath}");
                }
            }

            private bool ValidatePrefabs()
            {
                if (CameraManagerPrefab == null || GridPrefab == null || PlayerPrefab == null)
                {
                    Debug.LogError("[Frog Tools] One or more required prefabs are unassigned. Scene creation aborted.");

                    return false;
                }

                return true;
            }

            private static void InstantiatePrefab(GameObject prefabAsset, string undoName)
            {
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefabAsset);
                instance.transform.position = Vector3.zero;
                Undo.RegisterCreatedObjectUndo(instance, $"Instantiate {undoName}");
            }

            private static void EnsureDirectoryExists(string path)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    AssetDatabase.Refresh();
                }
            }
        }
    }
}