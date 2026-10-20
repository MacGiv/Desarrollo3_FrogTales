namespace FrogGame.Gameplay
{
    using UnityEngine;
    using UnityEngine.Tilemaps;

    /// <summary>
    /// Detects standing surface layer (Ground or Water) using a Box Gizmo positioned at the player's feet.
    /// </summary>
    public class PlayerWaterDetector : MonoBehaviour
    {
        [Header("Layer Detection")]
        [SerializeField] private LayerMask waterLayer;
        [SerializeField] private LayerMask groundLayer;

        [Header("Feet Detection Settings")]
        [SerializeField] private Vector2 feetOffset = new Vector2(0f, -0.35f);
        [SerializeField] private Vector2 boxSize = new Vector2(0.1f, 0.05f);
        [SerializeField] private Color gizmoColor = Color.yellow;

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

        public Vector2 GetFeetPosition() => (Vector2)transform.position + feetOffset;

        private void Update()
        {
            if (brain == null || brain.FSM == null || brain.FSM.CurrentState == null) return;

            if (brain.FSM.CurrentState is PlayerGrappleState || brain.FSM.CurrentState is PlayerDrownState)
            {
                return;
            }

            Vector2 feetPos = GetFeetPosition();

            Collider2D waterHit = Physics2D.OverlapBox(feetPos, boxSize, 0f, waterLayer);

            if (waterHit != null)
            {
                Vector3 closestPoint = waterHit.ClosestPoint(feetPos);
                Vector3 dirToWater = (closestPoint - (Vector3)feetPos).normalized;

                if (dirToWater == Vector3.zero && brain.MovementHandler != null)
                {
                    dirToWater = (Vector3)brain.MovementHandler.FacingDirection;
                }

                Vector3 pointInsideWater = closestPoint + (dirToWater * 0.2f);
                Vector3 waterCenter = pointInsideWater;

                if (currentGrid != null)
                {
                    Vector3Int waterCell = currentGrid.WorldToCell(pointInsideWater);

                    waterCenter = currentGrid.GetCellCenterWorld(waterCell);
                }

                brain.DrownState.SetTargetPositions(waterCenter, lastSafeTileCenter);
                brain.FSM.ChangeState(brain.DrownState);
                return;
            }

            if (currentGrid != null)
            {
                Vector3Int feetCellPos = currentGrid.WorldToCell(feetPos);

                if (waterTilemap != null && waterTilemap.HasTile(feetCellPos))
                {
                    Vector3 waterCenter = currentGrid.GetCellCenterWorld(feetCellPos);
                    brain.DrownState.SetTargetPositions(waterCenter, lastSafeTileCenter);
                    brain.FSM.ChangeState(brain.DrownState);
                    return;
                }

                Vector3Int playerCenterCell = currentGrid.WorldToCell(transform.position);

                if (playerCenterCell != currentCell)
                {
                    if (groundTilemap != null && groundTilemap.HasTile(playerCenterCell))
                    {
                        currentCell = playerCenterCell;
                        lastSafeTileCenter = currentGrid.GetCellCenterWorld(currentCell);
                    }
                }
            }
        }

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

                if ((tilemapLayer & groundLayer) != 0) groundTilemap = tilemap;
                if ((tilemapLayer & waterLayer) != 0) waterTilemap = tilemap;
            }
        }

        private void OnDrawGizmos()
        {
            Vector2 feetPos = GetFeetPosition();
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireCube(feetPos, boxSize);
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