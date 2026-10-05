namespace FrogGame.Gameplay
{
    using UnityEngine;
    using UnityEngine.Tilemaps;

    /// <summary>
    /// Detects standing surface layer (Ground or Water) using Tilemap references and tracks safe ground position.
    /// </summary>
    public class PlayerWaterDetector : MonoBehaviour
    {
        [Header("Layer Detection")]
        [SerializeField] private LayerMask waterLayer;
        [SerializeField] private LayerMask groundLayer;

        private Grid currentGrid;
        private Tilemap groundTilemap;
        private Tilemap waterTilemap;
        private PlayerBrain brain;

        private Vector3Int currentCell;
        private Vector3 lastSafeTileCenter;

        public Vector3 LastSafeTileCenter => lastSafeTileCenter;

        private void Awake() => brain = GetComponent<PlayerBrain>();

        private void Start()
        {
            InitializeGridReferences();

            if (currentGrid != null)
            {
                currentCell = currentGrid.WorldToCell(transform.position);
                lastSafeTileCenter = currentGrid.GetCellCenterWorld(currentCell);
            }
            else
            {
                lastSafeTileCenter = transform.position;
            }
        }

        private void Update()
        {
            if (brain == null || brain.FSM == null || brain.FSM.CurrentState == null) return;

            // Immune while grappling or already drowning
            if (brain.FSM.CurrentState is PlayerGrappleState || brain.FSM.CurrentState is PlayerDrownState)
            {
                return;
            }

            if (currentGrid == null) return;

            Vector3Int cellPos = currentGrid.WorldToCell(transform.position);

            // Water Detection via direct Tilemap query
            if (waterTilemap != null && waterTilemap.HasTile(cellPos))
            {
                Vector3 waterCenter = currentGrid.GetCellCenterWorld(cellPos);

                brain.DrownState.SetTargetPositions(waterCenter, lastSafeTileCenter);
                brain.FSM.ChangeState(brain.DrownState);

                return;
            }

            // Cell Change & Ground Save
            if (cellPos != currentCell)
            {
                currentCell = cellPos;

                // Query Tilemap directly without physics matrix overhead
                if (groundTilemap != null && groundTilemap.HasTile(currentCell))
                {
                    lastSafeTileCenter = currentGrid.GetCellCenterWorld(currentCell);
                }
            }
        }

        /// <summary>
        /// Registers grid and tilemap instances dynamically on initialization.
        /// </summary>
        private void InitializeGridReferences()
        {
            currentGrid = FindFirstObjectByType<Grid>();

            if (currentGrid == null)
            {
                Debug.LogWarning("[PlayerWaterDetector] No Grid instance found in scene!");

                return;
            }

            Tilemap[] allTilemaps = FindObjectsByType<Tilemap>(FindObjectsSortMode.None);

            foreach (Tilemap tilemap in allTilemaps)
            {
                int tilemapLayer = 1 << tilemap.gameObject.layer;

                if ((tilemapLayer & groundLayer) != 0)
                {
                    groundTilemap = tilemap;
                }

                if ((tilemapLayer & waterLayer) != 0)
                {
                    waterTilemap = tilemap;
                }
            }

            if (groundTilemap == null)
            {
                Debug.LogWarning("[PlayerWaterDetector] Ground Tilemap not found in scene!");
            }

            if (waterTilemap == null)
            {
                Debug.LogWarning("[PlayerWaterDetector] Water Tilemap not found in scene!");
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (currentGrid != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireCube(lastSafeTileCenter, currentGrid.cellSize * 0.9f);
            }
        }
    }
}