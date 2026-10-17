namespace FrogGame.Camera
{
    using System.Collections;
    using UnityEngine;
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

        private Room currentRoom;
        private bool isTransitioning = false;

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

        private void LateUpdate()
        {
            if (!isTransitioning && currentRoom != null && cameraTarget != null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    cameraTarget.position = currentRoom.GetClampedCameraPosition(player.transform.position, mainCamera);
                }
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

            PlayerBrain brain = playerTransform.GetComponent<PlayerBrain>();

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