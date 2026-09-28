namespace FrogGame.Gameplay
{
    using UnityEngine;

    /// <summary>
    /// Handles holding and dropping carried objects like puzzle boxes.
    /// </summary>
    public class PlayerCarryHandler : MonoBehaviour
    {
        [Header("Carry Settings")]
        [SerializeField] private Transform holdPoint; // Frog's head point
        [SerializeField] private float dropDistance = 1.0f; // Distance drop away from player pos
        [SerializeField] private LayerMask obstacleLayer; // Avoid dropping the box in a wall

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
            if (IsCarrying) return;

            currentCarriedBox = box;
            Transform boxTransform = box.transform;

            boxTransform.SetParent(holdPoint != null ? holdPoint : transform);
            boxTransform.localPosition = Vector3.zero;

            if (box.ItemCollider != null)
            {
                box.ItemCollider.enabled = false;
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

            // Verificar si hay pared en la posición de descarga
            Collider2D hitObstacle = Physics2D.OverlapCircle(dropPosition, 0.3f, obstacleLayer);
            if (hitObstacle != null)
            {
                Debug.LogWarning("[PlayerCarryHandler] Cannot drop box inside a wall!");
                return false;
            }

            // Desenganchar caja
            Transform boxTransform = currentCarriedBox.transform;
            boxTransform.SetParent(null);
            boxTransform.position = dropPosition;

            if (currentCarriedBox.ItemCollider != null)
            {
                currentCarriedBox.ItemCollider.enabled = true;
            }

            currentCarriedBox = null;
            return true;
        }
    }
}
