namespace FrogGame.Camera
{
    using System.Collections;
    using UnityEngine;
    using FrogGame.Gameplay;

    /// <summary>
    /// Manages room transitions, camera tracking, and dynamic look-ahead behavior for horizontal and vertical rooms.
    /// Gestiona transiciones de sala, seguimiento de cámara y comportamiento de avance (Look Ahead) dinámico.
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
        /// Calculates smooth camera look-ahead offset based on room orientation and player facing direction.
        /// Calcula el desplazamiento suave de la cámara según la orientación de la sala y la dirección del personaje.
        /// </summary>
        private Vector3 CalculateLookAheadPosition()
        {
            Vector3 playerPos = cachedPlayerBrain.transform.position;
            Vector2 facingDir = cachedPlayerBrain.MovementHandler != null ? cachedPlayerBrain.MovementHandler.FacingDirection : Vector2.zero;

            Vector2 targetOffset = Vector2.zero;

            switch (currentRoom.Type)
            {
                case RoomType.Horizontal:
                    targetOffset.x = facingDir.x * lookAheadDistance;
                    break;

                case RoomType.Vertical:
                    targetOffset.y = facingDir.y * lookAheadDistance;
                    break;

                case RoomType.Fixed:
                default:
                    targetOffset = Vector2.zero;
                    break;
            }

            currentLookAheadOffset = Vector2.MoveTowards(
                currentLookAheadOffset,
                targetOffset,
                lookAheadSpeed * Time.deltaTime
            );

            return playerPos + (Vector3)currentLookAheadOffset;
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
            currentLookAheadOffset = Vector2.zero; // Reset look-ahead during room transition

            PlayerBrain brain = playerTransform.GetComponent<PlayerBrain>();
            if (brain != null) cachedPlayerBrain = brain;

            // 1. Lock inputs, stop movement, and apply invincibility
            if (brain != null)
            {
                brain.InputHandler?.DisableInput();
                brain.MovementHandler?.Stop();
                brain.HealthSystem?.TriggerInvincibility(roomTransitionInvincibility);
            }

            // 2. Calculate cardinal entry direction (N, S, E, W)
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

            // 3. Perform BoxCast to calculate safe target position without hitting Obstacles layer
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

            // 4. Calculate initial target camera position for the new room
            Vector3 targetCamPos = currentRoom.GetClampedCameraPosition(playerTargetPos, mainCamera);

            // 5. Smoothly transition camera and nudge player
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

            // 6. Unlock input on completion
            if (brain != null)
            {
                brain.InputHandler?.EnableInput();
            }

            isTransitioning = false;
        }
    }
}