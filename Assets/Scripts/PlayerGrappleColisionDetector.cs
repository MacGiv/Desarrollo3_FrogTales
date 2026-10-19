namespace FrogGame.Gameplay
{
    using UnityEngine;

    [RequireComponent(typeof(Collider2D))]
    public class PlayerGrappleCollisionDetector : MonoBehaviour
    {
        [Header("Collision Detection")]
        [SerializeField] private LayerMask collisionLayers;
        [SerializeField] private Vector2 boxSize = new Vector2(0.65f, 0.25f);
        [SerializeField] private float detectionDistance = 0.5f;
        [SerializeField] private Color gizmoColor = Color.green;

        private PlayerMovementHandler movementHandler;

        private void Awake() => movementHandler = GetComponent<PlayerMovementHandler>();

        public Vector2 GetBoxCenter()
        {
            Vector2 direction = GetFacingDirection();

            return (Vector2)transform.position + (direction * detectionDistance);
        }

        public float GetBoxAngle()
        {
            Vector2 dir = GetFacingDirection();

            return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        }

        private Vector2 GetFacingDirection()
        {
            if (movementHandler != null && movementHandler.FacingDirection != Vector2.zero)
            {
                return movementHandler.FacingDirection;
            }

            return Vector2.down;
        }

        public bool CanGrapple()
        {
            Vector2 boxCenter = GetBoxCenter();
            float angle = GetBoxAngle();

            Collider2D hit = Physics2D.OverlapBox(boxCenter, boxSize, angle, collisionLayers);

            return hit == null;
        }

        private void OnDrawGizmos()
        {
            if (movementHandler == null)
            {
                movementHandler = GetComponent<PlayerMovementHandler>();
            }

            Vector2 center = GetBoxCenter();
            float angle = GetBoxAngle();

            Gizmos.color = gizmoColor;
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(center, Quaternion.Euler(0f, 0f, angle), Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, boxSize);
            Gizmos.matrix = oldMatrix;
        }
    }
}