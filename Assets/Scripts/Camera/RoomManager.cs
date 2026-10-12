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
        [SerializeField] private float transitionSpeed = 15f; // Velocidad fija de la cámara
        [SerializeField] private float playerNudgeDistance = 1.2f; // Empujón al jugador para no re-activar el trigger

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
            // En gameplay normal (sin transición), actualiza el CameraTarget siguiendo al Player
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

            // 1. Bloquear input y detener movimiento del personaje
            if (brain != null)
            {
                brain.InputHandler?.DisableInput();
                brain.MovementHandler?.Stop();
            }

            // 2. Calcular dirección de entrada y desplazar suavemente al jugador
            Vector2 entryDirection = (playerTransform.position - cameraTarget.position).normalized;
            if (entryDirection == Vector2.zero) entryDirection = Vector2.right;

            Vector3 playerTargetPos = playerTransform.position + (Vector3)(entryDirection * playerNudgeDistance);

            // 3. Posición objetivo inicial de la cámara en la nueva sala
            Vector3 targetCamPos = currentRoom.GetClampedCameraPosition(playerTargetPos, mainCamera);

            // 4. Mover la cámara a velocidad constante (Mega Man X style)
            while (Vector3.Distance(cameraTarget.position, targetCamPos) > 0.05f)
            {
                cameraTarget.position = Vector3.MoveTowards(
                    cameraTarget.position,
                    targetCamPos,
                    transitionSpeed * Time.deltaTime
                );

                // Opcional: desplazar ligeramente al jugador durante la transición
                playerTransform.position = Vector3.MoveTowards(
                    playerTransform.position,
                    playerTargetPos,
                    (transitionSpeed * 0.3f) * Time.deltaTime
                );

                yield return null;
            }

            cameraTarget.position = targetCamPos;
            playerTransform.position = playerTargetPos;

            // 5. Desbloquear input al finalizar
            if (brain != null)
            {
                brain.InputHandler?.EnableInput();
            }

            isTransitioning = false;
        }
    }
}