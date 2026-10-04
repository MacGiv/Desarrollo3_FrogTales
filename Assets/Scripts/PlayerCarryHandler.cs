namespace FrogGame.Gameplay
{
    using UnityEngine;

    /// <summary>
    /// Handles holding and dropping carried objects like puzzle boxes.
    /// </summary>
    public class PlayerCarryHandler : MonoBehaviour
    {
        [Header("Carry Settings")]
        [SerializeField] private Transform holdPoint; // Point above frog's head
        [SerializeField] private float dropDistance = 1.0f; // Distance in front of player
        [SerializeField] private LayerMask obstacleLayer; // Avoid dropping inside walls

        private GrabbableItem currentCarriedBox = null;
        private PlayerMovementHandler movementHandler;

        public bool IsCarrying => currentCarriedBox != null;

        private void Awake()
        {
            movementHandler = GetComponent<PlayerMovementHandler>();
        }

        /// <summary>
        /// Attaches the box to the player's hold point above its head.
        /// </summary>
        public void AttachBox(GrabbableItem box)
        {
            if (IsCarrying || box == null) return;

            currentCarriedBox = box;
            Transform boxTransform = box.transform;

            // 1. Assign hold point
            Transform targetParent = holdPoint != null ? holdPoint : transform;
            boxTransform.SetParent(targetParent);

            // 2. Reset local position and rotation to the center of HoldPoint
            boxTransform.localPosition = Vector3.zero;
            boxTransform.localRotation = Quaternion.identity;

            // 3. Deactivate collider
            if (box.ItemCollider != null)
            {
                box.ItemCollider.enabled = false;
            }

            // 4. Deactivate physics
            if (box.TryGetComponent<Rigidbody2D>(out var rb))
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.simulated = false; // Stop rb simulation
            }
        }

        /// <summary>
        /// Drops the carried box immediately in front of the player's facing direction.
        /// </summary>
        public bool TryDropBox()
        {
            if (!IsCarrying) return false;

            Vector2 facingDir = movementHandler != null ? movementHandler.FacingDirection : Vector2.down;
            Vector2 dropPosition = (Vector2)transform.position + (facingDir * dropDistance);

            // Check for obstacles
            Collider2D hitObstacle = Physics2D.OverlapCircle(dropPosition, 0.3f, obstacleLayer);
            if (hitObstacle != null)
            {
                Debug.LogWarning("[PlayerCarryHandler] Cannot drop box inside a wall!");
                return false;
            }

            // Remove from hierarchy
            Transform boxTransform = currentCarriedBox.transform;
            boxTransform.SetParent(null);
            boxTransform.position = dropPosition;

            // Reactivate collider
            if (currentCarriedBox.ItemCollider != null)
            {
                currentCarriedBox.ItemCollider.enabled = true;
            }

            // Reactivate physiscs
            if (currentCarriedBox.TryGetComponent<Rigidbody2D>(out var rb))
            {
                rb.simulated = true;
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }

            currentCarriedBox = null;
            return true;
        }
    }
}