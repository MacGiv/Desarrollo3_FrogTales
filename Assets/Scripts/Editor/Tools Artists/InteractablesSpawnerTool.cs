namespace FrogGame.Editor
{
    using UnityEditor;
    using UnityEngine;

    public static class InteractablesSpawnerTool
    {
        private const string MENU_NAME = "Artist Tools";

        private static class MenuPriority
        {
            public const int GRAPPLE_POINT = 20;
            public const int ITEM_HEALTH = 21;
            public const int PICKUP_ARROW = 22;
            public const int BOX = 23;
            public const int SWITCH = 24;
            public const int DOOR = 25;
        }

        private static void SpawnInteractable(string prefabPath, string interactableName)
        {
            GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefabAsset == null)
            {
                Debug.LogError($"[Frog Tools] Required prefab missing at path: '{prefabPath}'. Spawning aborted.");

                return;
            }

            InstantiateInScene(prefabAsset, interactableName);
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

        [MenuItem("Frog Tools/" + MENU_NAME + "/Create/New Interactable/GrapplePoint", false, MenuPriority.GRAPPLE_POINT)]
        public static void SpawnGrapplePoint() => SpawnInteractable("Assets/Prefabs/GrapplePoint.prefab", "GrapplePoint");

        [MenuItem("Frog Tools/" + MENU_NAME + "/Create/New Interactable/ItemHealth", false, MenuPriority.ITEM_HEALTH)]
        public static void SpawnItemHealth() => SpawnInteractable("Assets/Prefabs/ItemHealth.prefab", "ItemHealth");

        [MenuItem("Frog Tools/" + MENU_NAME + "/Create/New Interactable/PickupArrow", false, MenuPriority.PICKUP_ARROW)]
        public static void SpawnPickupArrow() => SpawnInteractable("Assets/Prefabs/PickupArrow.prefab", "PickupArrow");

        [MenuItem("Frog Tools/" + MENU_NAME + "/Create/New Interactable/Box", false, MenuPriority.BOX)]
        public static void SpawnBox() => SpawnInteractable("Assets/Prefabs/Box.prefab", "Box");

        [MenuItem("Frog Tools/" + MENU_NAME + "/Create/New Interactable/Switch", false, MenuPriority.SWITCH)]
        public static void SpawnSwitch() => SpawnInteractable("Assets/Prefabs/Switch.prefab", "Switch");

        [MenuItem("Frog Tools/" + MENU_NAME + "/Create/New Interactable/Door", false, MenuPriority.DOOR)]
        public static void SpawnDoor() => SpawnInteractable("Assets/Prefabs/Door.prefab", "Door");
    }
}