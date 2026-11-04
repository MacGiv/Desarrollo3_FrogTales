namespace FrogGame.Gameplay
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Trigger-based moving platform for top-down gameplay.
    /// Supports bi-directional travel (Point A <-> Point B) and resets to the current trip's origin shore upon player drowning.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class MovingPlatform : MonoBehaviour
    {
        public enum PlatformMode { Always, OnTrigger }

        [Header("Movement Settings")]
        [SerializeField] private Transform pointA;
        [SerializeField] private Transform pointB;
        [SerializeField] private float speed = 2.5f;
        [SerializeField] private float waitTimeAtWaypoints = 0.5f;
        [SerializeField] private PlatformMode mode = PlatformMode.Always;

        [Header("3x3 Center Tile Detection")]
        [Tooltip("Max distance from platform center (0,0) to consider the player is on the center tile.")]
        [SerializeField] private float centerTileRadius = 0.7f;

        private Rigidbody2D rb;
        private Vector3 targetPosition;
        private Vector3 currentOrigin; // Dynamic origin shore for current trip
        private Vector3 lastPlatformPosition;

        private Transform playerTransform;
        private bool isPlayerOnPlatform = false;
        private bool isMoving = false;
        private bool isWaiting = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.useFullKinematicContacts = true;
        }

        private void OnEnable()
        {
            PlayerDrownState.OnPlayerDrowned += HandlePlayerDrowned;
        }

        private void OnDisable()
        {
            PlayerDrownState.OnPlayerDrowned -= HandlePlayerDrowned;
        }

        private void Start()
        {
            if (pointA != null && pointB != null)
            {
                currentOrigin = pointA.position;
                targetPosition = pointB.position;
            }

            ResetToCurrentOrigin();
        }

        private void HandlePlayerDrowned()
        {
            isPlayerOnPlatform = false;
            playerTransform = null;

            if (mode == PlatformMode.OnTrigger)
            {
                ResetToCurrentOrigin();
            }
        }

        /// <summary>
        /// Resets the platform to the starting shore of its current journey leg.
        /// </summary>
        public void ResetToCurrentOrigin()
        {
            StopAllCoroutines();
            isWaiting = false;
            isMoving = (mode == PlatformMode.Always);

            transform.position = currentOrigin;
            rb.position = currentOrigin;
            lastPlatformPosition = currentOrigin;
        }

        private void FixedUpdate()
        {
            Vector3 currentPos = transform.position;

            if (isMoving && !isWaiting && pointA != null && pointB != null)
            {
                Vector3 targetPos = targetPosition;
                Vector3 newPos = Vector3.MoveTowards(currentPos, targetPos, speed * Time.fixedDeltaTime);

                rb.MovePosition(newPos);

                if (Vector3.Distance(newPos, targetPos) < 0.02f)
                {
                    StartCoroutine(WaitAndSwitchTarget());
                }
            }

            Vector3 platformDelta = transform.position - lastPlatformPosition;

            if (isPlayerOnPlatform && playerTransform != null && platformDelta != Vector3.zero)
            {
                playerTransform.position += platformDelta;
            }

            lastPlatformPosition = transform.position;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerTransform = other.transform;
                isPlayerOnPlatform = true;
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerTransform = other.transform;
                isPlayerOnPlatform = true;

                if (mode == PlatformMode.OnTrigger && !isMoving && !isWaiting)
                {
                    float distanceToCenter = Vector2.Distance(other.transform.position, transform.position);

                    if (distanceToCenter <= centerTileRadius)
                    {
                        isMoving = true;
                    }
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerOnPlatform = false;
                playerTransform = null;
            }
        }

        private IEnumerator WaitAndSwitchTarget()
        {
            isWaiting = true;

            if (mode == PlatformMode.OnTrigger)
            {
                isMoving = false;
            }

            yield return new WaitForSeconds(waitTimeAtWaypoints);

            // The reached waypoint becomes the new origin for the return trip
            currentOrigin = targetPosition;
            targetPosition = (targetPosition == pointA.position) ? pointB.position : pointA.position;
            isWaiting = false;
        }

        private void OnDrawGizmos()
        {
            if (pointA != null && pointB != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(pointA.position, pointB.position);
                Gizmos.DrawWireSphere(pointA.position, 0.3f);
                Gizmos.DrawWireSphere(pointB.position, 0.3f);
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, centerTileRadius);
        }
    }
}