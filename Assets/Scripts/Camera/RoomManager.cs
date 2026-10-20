namespace FrogGame.Camera
{
    using System.Collections;
    using UnityEngine;
    using Unity.Cinemachine;
    using FrogGame.Gameplay;

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
                    // Only update X memory when actively facing horizontally
                    if (facingDir.x != 0f)
                    {
                        lastLookAheadDirX = facingDir.x;
                    }

                    targetOffset.x = lastLookAheadDirX * lookAheadDistance;
                    break;

                case RoomType.Vertical:
                    // Only update Y memory when actively facing vertically
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

            Vector2 rawDir = (playerTransform.position - cameraTarget.position).normalized;
            Vector2 entryDirection = Vector2.right;

            if (rawDir != Vector2.zero)
            {
                if (Mathf.Abs(rawDir.x) >= Mathf.Abs(rawDir.y))
                {
                    entryDirection = new Vector2(Mathf.Sign(rawDir.x), 0f);
                }
                else
                {
                    entryDirection = new Vector2(0f, Mathf.Sign(rawDir.y));
                }
            }

            float safeDistance = playerNudgeDistance;
            Collider2D playerCollider = playerTransform.GetComponent<Collider2D>();
            Vector2 boxSize = playerCollider != null ? (Vector2)playerCollider.bounds.size : new Vector2(0.8f, 0.8f);

            RaycastHit2D hit = Physics2D.BoxCast(
                playerTransform.position,
                boxSize,
                0f,
                entryDirection,
                playerNudgeDistance,
                obstacleLayer
            );

            if (hit.collider != null)
            {
                safeDistance = Mathf.Max(0f, hit.distance - 0.1f);
            }

            Vector3 playerTargetPos = playerTransform.position + (Vector3)(entryDirection * safeDistance);
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
    }
}