namespace FrogGame.Gameplay
{
    using System.Collections;
    using UnityEngine;
    using FrogGame.Camera;
    using FrogGame.UI;

    /// <summary>
    /// Handles Gameboy-style interior transitions with fade effects, instant position/camera teleportation, safe positioning, and room updates.
    /// Maneja transiciones de interior estilo Gameboy con efectos de fade, teletransporte instantáneo de posición/cámara y cambio de sala.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class TeleportDoor : MonoBehaviour
    {
        [Header("Teleport Destination")]
        [SerializeField] private Transform targetSpawnPoint;
        [SerializeField] private Room destinationRoom;
        [SerializeField] private Vector2 exitFacingDirection = Vector2.up;

        [Header("Transition Settings")]
        [SerializeField] private float fadeDuration = 0.3f;
        [SerializeField] private float blackHoldTime = 0.05f;

        [Header("Collision Prevention")]
        [SerializeField] private LayerMask obstacleLayer;

        private bool isTransitioning = false;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (isTransitioning || !collision.CompareTag("Player")) return;

            PlayerBrain brain = collision.GetComponent<PlayerBrain>();
            if (brain != null && targetSpawnPoint != null)
            {
                StartCoroutine(TransitionRoutine(brain));
            }
        }

        private IEnumerator TransitionRoutine(PlayerBrain brain)
        {
            isTransitioning = true;

            // 1. Lock input & halt player physics
            brain.InputHandler?.DisableInput();
            brain.MovementHandler?.Stop();

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayEnvDoorsOpen(gameObject);
            }

            // 2. Fade to black
            if (ScreenFader.Instance != null)
            {
                yield return ScreenFader.Instance.FadeToBlackRoutine(fadeDuration);
            }

            // 3. Calculate safe spawn position to prevent spawning inside Obstacles layer
            Vector3 safeSpawnPos = GetSafeSpawnPosition(brain);
            brain.transform.position = safeSpawnPos;

            // Set facing direction upon exit
            if (brain.MovementHandler != null)
            {
                brain.MovementHandler.SetFacingDirection(exitFacingDirection);
            }

            // 4. Instant camera & target snap to destination room
            if (destinationRoom != null && RoomManager.Instance != null)
            {
                RoomManager.Instance.SnapToRoom(destinationRoom, safeSpawnPos);
            }

            // 5. Wait 1 frame so Unity updates physics/camera rendering while 100% black
            yield return null;
            if (blackHoldTime > 0)
            {
                yield return new WaitForSeconds(blackHoldTime);
            }

            // 6. Fade back to clear
            if (ScreenFader.Instance != null)
            {
                yield return ScreenFader.Instance.FadeToClearRoutine(fadeDuration);
            }

            // 7. Restore player control
            brain.InputHandler?.EnableInput();
            isTransitioning = false;
        }

        /// <summary>
        /// Calculates a safe spawn position ensuring the player does not spawn inside an obstacle.
        /// </summary>
        private Vector3 GetSafeSpawnPosition(PlayerBrain brain)
        {
            Vector3 rawPos = targetSpawnPoint.position;
            Collider2D playerCol = brain.GetComponent<Collider2D>();
            Vector2 boxSize = playerCol != null ? (Vector2)playerCol.bounds.size : new Vector2(0.8f, 0.8f);

            // Check if target position overlaps with any obstacle
            Collider2D overlap = Physics2D.OverlapBox(rawPos, boxSize * 0.9f, 0f, obstacleLayer);
            if (overlap != null)
            {
                // Nudge position along exitFacingDirection to clear obstacle
                RaycastHit2D hit = Physics2D.BoxCast(rawPos, boxSize, 0f, exitFacingDirection, 1.5f, obstacleLayer);
                if (hit.collider == null)
                {
                    rawPos += (Vector3)(exitFacingDirection.normalized * 0.5f);
                }
            }

            return rawPos;
        }

        private void OnDrawGizmosSelected()
        {
            if (targetSpawnPoint != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(targetSpawnPoint.position, 0.4f);
                Gizmos.DrawLine(transform.position, targetSpawnPoint.position);
            }
        }
    }
}