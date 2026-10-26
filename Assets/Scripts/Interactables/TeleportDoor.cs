namespace FrogGame.Gameplay
{
    using System.Collections;
    using UnityEngine;
    using FrogGame.Camera;
    using FrogGame.UI;

    /// <summary>
    /// Handles Gameboy-style interior transitions with fade effects, instant position/camera teleportation, and room updates.
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

            // 3. Teleport player position behind black screen
            brain.transform.position = targetSpawnPoint.position;

            // 4. Instant camera & target snap to destination room
            if (destinationRoom != null && RoomManager.Instance != null)
            {
                RoomManager.Instance.SnapToRoom(destinationRoom, targetSpawnPoint.position);
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