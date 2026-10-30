namespace FrogGame.Camera
{
    using System.Collections;
    using UnityEngine;
    using Unity.Cinemachine;
    using FrogGame.Gameplay;

    /// <summary>
    /// Manages room-based camera clamping, look-ahead tracking, and safe collision-free player transitions between rooms using closest edge detection.
    /// </summary>
    public class RoomManager : MonoBehaviour
    {
        public static RoomManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private Transform cameraTarget;
        [SerializeField] private UnityEngine.Camera mainCamera;

        [Header("Transition Settings")]
        [SerializeField] private float transitionSpeed = 15f;
        [SerializeField] private float playerNudgeDistance = 1.2f;
        [SerializeField] private float roomTransitionInvincibility = 1.5f;

        [Header("Collision Prevention")]
        [SerializeField] private LayerMask obstacleLayer;

        [Header("Look Ahead Settings")]
        [SerializeField] private float lookAheadDistance = 2.5f;
        [SerializeField] private float lookAheadSpeed = 4.0f;

        private Room currentRoom;
        private bool isTransitioning = false;
        private PlayerBrain cachedPlayerBrain;
        private Vector2 currentLookAheadOffset;

        // Sticky directional tracking for look-ahead stability
        private float lastLookAheadDirX = 0f;
        private float lastLookAheadDirY = 0f;

        public bool IsTransitioning => isTransitioning;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (mainCamera == null) mainCamera = UnityEngine.Camera.main;
        }

        private void Start()
        {
            CachePlayerReference();
        }

        private void LateUpdate()
        {
            if (isTransitioning || currentRoom == null || cameraTarget == null) return;

            if (cachedPlayerBrain == null)
            {
                CachePlayerReference();
                if (cachedPlayerBrain == null) return;
            }

            Vector3 targetPosition = CalculateLookAheadPosition();
            cameraTarget.position = currentRoom.GetClampedCameraPosition(targetPosition, mainCamera);
        }

        /// <summary>
        /// Calculates camera look-ahead offset, permanently preserving the last directional choice along the room's primary axis.
        /// </summary>
        private Vector3 CalculateLookAheadPosition()
        {
            Vector3 playerPos = cachedPlayerBrain.transform.position;
            Vector2 facingDir = cachedPlayerBrain.MovementHandler != null ? cachedPlayerBrain.MovementHandler.FacingDirection : Vector2.zero;

            Vector2 targetOffset = Vector2.zero;

            switch (currentRoom.Type)
            {
                case RoomType.Horizontal:
                    if (facingDir.x != 0f)
                    {
                        lastLookAheadDirX = facingDir.x;
                    }
                    targetOffset.x = lastLookAheadDirX * lookAheadDistance;
                    break;

                case RoomType.Vertical:
                    if (facingDir.y != 0f)
                    {
                        lastLookAheadDirY = facingDir.y;
                    }
                    targetOffset.y = lastLookAheadDirY * lookAheadDistance;
                    break;

                case RoomType.Fixed:
                default:
                    lastLookAheadDirX = 0f;
                    lastLookAheadDirY = 0f;
                    currentLookAheadOffset = Vector2.zero;
                    return playerPos;
            }

            currentLookAheadOffset = Vector2.MoveTowards(
                currentLookAheadOffset,
                targetOffset,
                lookAheadSpeed * Time.deltaTime
            );

            return playerPos + (Vector3)currentLookAheadOffset;
        }

        /// <summary>
        /// Instantly snaps cameraTarget, resets look-ahead memory, and triggers Cinemachine target warping.
        /// </summary>
        public void SnapToRoom(Room newRoom, Vector3 playerPosition)
        {
            if (newRoom == null) return;

            currentRoom = newRoom;
            currentLookAheadOffset = Vector2.zero;
            lastLookAheadDirX = 0f;
            lastLookAheadDirY = 0f;

            if (cameraTarget != null && mainCamera != null)
            {
                Vector3 oldTargetPos = cameraTarget.position;
                Vector3 targetCamPos = currentRoom.GetClampedCameraPosition(playerPosition, mainCamera);
                Vector3 delta = targetCamPos - oldTargetPos;

                cameraTarget.position = targetCamPos;
                CinemachineCore.OnTargetObjectWarped(cameraTarget, delta);
            }
        }

        private void CachePlayerReference()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                cachedPlayerBrain = playerObj.GetComponent<PlayerBrain>();
            }
        }

        public void RequestRoomChange(Room newRoom, Transform playerTransform)
        {
            if (isTransitioning || currentRoom == newRoom) return;

            StartCoroutine(TransitionRoutine(newRoom, playerTransform));
        }

        private IEnumerator TransitionRoutine(Room newRoom, Transform playerTransform)
        {
            isTransitioning = true;
            currentRoom = newRoom;
            currentLookAheadOffset = Vector2.zero;
            lastLookAheadDirX = 0f;
            lastLookAheadDirY = 0f;

            PlayerBrain brain = playerTransform.GetComponent<PlayerBrain>();
            if (brain != null) cachedPlayerBrain = brain;

            if (brain != null)
            {
                brain.InputHandler?.DisableInput();
                brain.MovementHandler?.Stop();
                brain.HealthSystem?.TriggerInvincibility(roomTransitionInvincibility);
            }

            // 1. Calculate direction deterministically based on the closest edge of the new room collider
            Vector2 entryDirection = DetermineEntryDirection(playerTransform.position, newRoom);

            Collider2D playerCollider = playerTransform.GetComponent<Collider2D>();
            Vector2 playerSize = playerCollider != null ? (Vector2)playerCollider.bounds.size : new Vector2(0.8f, 0.8f);

            // 2. Calculate safe target position inside the new room avoiding Obstacles
            Vector3 playerTargetPos = GetSafePlayerPosition(playerTransform.position, entryDirection, playerNudgeDistance, playerSize, newRoom);
            Vector3 targetCamPos = currentRoom.GetClampedCameraPosition(playerTargetPos, mainCamera);

            while (Vector3.Distance(cameraTarget.position, targetCamPos) > 0.05f)
            {
                cameraTarget.position = Vector3.MoveTowards(
                    cameraTarget.position,
                    targetCamPos,
                    transitionSpeed * Time.deltaTime
                );

                playerTransform.position = Vector3.MoveTowards(
                    playerTransform.position,
                    playerTargetPos,
                    (transitionSpeed * 0.3f) * Time.deltaTime
                );

                yield return null;
            }

            cameraTarget.position = targetCamPos;
            playerTransform.position = playerTargetPos;

            if (brain != null)
            {
                brain.InputHandler?.EnableInput();
            }

            isTransitioning = false;
        }

        /// <summary>
        /// Calculates entry direction by identifying which boundary edge of the target room the player crossed.
        /// </summary>
        private Vector2 DetermineEntryDirection(Vector3 playerPos, Room newRoom)
        {
            if (newRoom == null || newRoom.Collider == null) return Vector2.up;

            Bounds bounds = newRoom.Collider.bounds;

            // Absolute distances to each of the 4 borders of the destination room
            float distLeft = Mathf.Abs(playerPos.x - bounds.min.x);
            float distRight = Mathf.Abs(playerPos.x - bounds.max.x);
            float distBottom = Mathf.Abs(playerPos.y - bounds.min.y);
            float distTop = Mathf.Abs(playerPos.y - bounds.max.y);

            // Find the closest border edge
            float minDist = distLeft;
            Vector2 entryDirection = Vector2.right; // Crossed Left edge -> Move Right

            if (distRight < minDist)
            {
                minDist = distRight;
                entryDirection = Vector2.left; // Crossed Right edge -> Move Left
            }
            if (distBottom < minDist)
            {
                minDist = distBottom;
                entryDirection = Vector2.up; // Crossed Bottom edge -> Move Up
            }
            if (distTop < minDist)
            {
                minDist = distTop;
                entryDirection = Vector2.down; // Crossed Top edge -> Move Down
            }

            return entryDirection;
        }

        /// <summary>
        /// Places the player safely inside the new room bounds and adjusts orthogonally if overlapping walls.
        /// </summary>
        private Vector3 GetSafePlayerPosition(Vector3 startPos, Vector2 direction, float desiredDistance, Vector2 playerSize, Room newRoom)
        {
            Vector2 skinSize = new Vector2(Mathf.Max(0.1f, playerSize.x - 0.04f), Mathf.Max(0.1f, playerSize.y - 0.04f));
            Bounds roomBounds = newRoom.Collider.bounds;

            Vector3 candidatePos = startPos;

            // Move candidate position past the crossed boundary into the new room
            if (direction == Vector2.right)
            {
                candidatePos.x = roomBounds.min.x + desiredDistance;
            }
            else if (direction == Vector2.left)
            {
                candidatePos.x = roomBounds.max.x - desiredDistance;
            }
            else if (direction == Vector2.up)
            {
                candidatePos.y = roomBounds.min.y + desiredDistance;
            }
            else if (direction == Vector2.down)
            {
                candidatePos.y = roomBounds.max.y - desiredDistance;
            }

            // Clamp candidate position to ensure it is strictly within the room collider
            candidatePos.x = Mathf.Clamp(candidatePos.x, roomBounds.min.x + (playerSize.x / 2f), roomBounds.max.x - (playerSize.x / 2f));
            candidatePos.y = Mathf.Clamp(candidatePos.y, roomBounds.min.y + (playerSize.y / 2f), roomBounds.max.y - (playerSize.y / 2f));

            // Depenetration check against Obstacles layer
            if (Physics2D.OverlapBox(candidatePos, skinSize, 0f, obstacleLayer) != null)
            {
                int maxSteps = 12;
                float stepSize = 0.1f;

                for (int i = 1; i <= maxSteps; i++)
                {
                    float offset = stepSize * i;

                    if (direction.x != 0f) // Moving horizontally -> Check Up and Down
                    {
                        Vector3 testUp = candidatePos + Vector3.up * offset;
                        if (Physics2D.OverlapBox(testUp, skinSize, 0f, obstacleLayer) == null && roomBounds.Contains(testUp))
                        {
                            return testUp;
                        }

                        Vector3 testDown = candidatePos + Vector3.down * offset;
                        if (Physics2D.OverlapBox(testDown, skinSize, 0f, obstacleLayer) == null && roomBounds.Contains(testDown))
                        {
                            return testDown;
                        }
                    }
                    else // Moving vertically -> Check Left and Right
                    {
                        Vector3 testLeft = candidatePos + Vector3.left * offset;
                        if (Physics2D.OverlapBox(testLeft, skinSize, 0f, obstacleLayer) == null && roomBounds.Contains(testLeft))
                        {
                            return testLeft;
                        }

                        Vector3 testRight = candidatePos + Vector3.right * offset;
                        if (Physics2D.OverlapBox(testRight, skinSize, 0f, obstacleLayer) == null && roomBounds.Contains(testRight))
                        {
                            return testRight;
                        }
                    }
                }
            }

            return candidatePos;
        }
    }
}