namespace FrogGame.Editor
{
    using UnityEditor;
    using UnityEngine;

    public static class EntitySpawnerTool
    {
        private const string MENU_NAME = "Artist Tools";

        private static class MenuPriority
        {
            public const int PLAYER = 1;
            public const int FOX = 2;
            public const int FOX_SHIELDED = 3;
            public const int SMALL_OWL = 4;
        }

        private static void SpawnEntity(string prefabPath, string entityName)
        {
            GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefabAsset == null)
            {
                Debug.LogError($"[Frog Tools] Required prefab missing at path: '{prefabPath}'. Spawning aborted.");

                return;
            }

            InstantiateInScene(prefabAsset, entityName);
        }

        private static void InstantiateInScene(GameObject prefabAsset, string undoName)
        {
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefabAsset);

            if (SceneView.lastActiveSceneView != null)
            {
                Vector3 spawnPosition = SceneView.lastActiveSceneView.pivot;
                spawnPosition.z = 0f;
                instance.transform.position = spawnPosition;
            }
            else
            {
                instance.transform.position = Vector3.zero;
            }

            Selection.activeGameObject = instance;
            Undo.RegisterCreatedObjectUndo(instance, $"Spawn {undoName}");
            Debug.Log($"[Frog Tools] Successfully spawned '{undoName}' in the active scene.");
        }

        [MenuItem("Frog Tools/" + MENU_NAME + "/Create/New Entity/Player", false, MenuPriority.PLAYER)]
        public static void SpawnPlayer() => SpawnEntity("Assets/Prefabs/Player.prefab", "Player");

        [MenuItem("Frog Tools/" + MENU_NAME + "/Create/New Entity/Fox", false, MenuPriority.FOX)]
        public static void SpawnFox() => SpawnEntity("Assets/Prefabs/Fox.prefab", "Fox");

        [MenuItem("Frog Tools/" + MENU_NAME + "/Create/New Entity/FoxShielded", false, MenuPriority.FOX_SHIELDED)]
        public static void SpawnFoxShielded() => SpawnEntity("Assets/Prefabs/FoxShielded.prefab", "FoxShielded");

        [MenuItem("Frog Tools/" + MENU_NAME + "/Create/New Entity/SmallOwl", false, MenuPriority.SMALL_OWL)]
        public static void SpawnSmallOwl() => SpawnEntity("Assets/Prefabs/SmallOwl.prefab", "SmallOwl");
    }
}