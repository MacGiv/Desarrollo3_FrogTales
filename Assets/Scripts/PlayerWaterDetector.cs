namespace FrogGame.Gameplay
{
    using UnityEngine;

    /// <summary>
    /// Detects standing surface layer (Ground or Water) and tracks safe ground position.
    /// </summary>
    public class PlayerWaterDetector : MonoBehaviour
    {
        [Header("Layer Detection")]
        [SerializeField] private LayerMask waterLayer;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private Vector2 checkSize = new Vector2(0.05f, 0.05f);

        private Grid currentGrid;
        private PlayerBrain brain;
        private Vector3 lastSafeTileCenter;

        public Vector3 LastSafeTileCenter => lastSafeTileCenter;

        private void Awake() => brain = GetComponent<PlayerBrain>();

        private void Start()
        {
            FindGridReference();

            lastSafeTileCenter = GetCurrentTileCenter();
        }

        private void Update()
        {
            if (brain == null || brain.FSM == null) return;

            // Immune while grappling or already drowning
            if (brain.FSM.CurrentState is PlayerGrappleState || brain.FSM.CurrentState is PlayerDrownState)
            {
                return;
            }

            Vector2 checkPosition = transform.position;

            // Check Water Layer
            Collider2D waterHit = Physics2D.OverlapBox(checkPosition, checkSize, 0f, waterLayer);

            if (waterHit != null)
            {
                Vector3 waterCenter = GetCurrentTileCenter();

                brain.DrownState.SetTargetPositions(waterCenter, lastSafeTileCenter);
                brain.FSM.ChangeState(brain.DrownState);

                return;
            }

            // Check Ground Layer and update safe tile center dynamically
            Collider2D groundHit = Physics2D.OverlapBox(checkPosition, checkSize, 0f, groundLayer);

            if (groundHit != null)
            {
                lastSafeTileCenter = GetCurrentTileCenter();
            }
        }

        /// <summary>
        /// Calculates the center world position of the tile the player is currently over.
        /// </summary>
        public Vector3 GetCurrentTileCenter()
        {
            if (currentGrid == null)
            {
                FindGridReference();

                if (currentGrid == null) return transform.position;
            }

            Vector3Int cellPosition = currentGrid.WorldToCell(transform.position);

            return currentGrid.GetCellCenterWorld(cellPosition);
        }

        private void FindGridReference()
        {
            currentGrid = FindFirstObjectByType<Grid>();

            if (currentGrid == null)
            {
                Debug.LogWarning("[PlayerWaterDetector] No Grid instance found in scene!");
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, checkSize);
        }
    }
}