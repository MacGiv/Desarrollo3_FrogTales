namespace FrogGame.Gameplay
{
    using System.Collections;
    using UnityEngine;
    using FrogGame.Core;

    /// <summary>
    /// Handles the drowning sequence, sprite rotation, damage application post-respawn, and input locking.
    /// </summary>
    public class PlayerDrownState : IState
    {
        private readonly PlayerBrain brain;

        private Vector3 waterTileCenter;
        private Vector3 safeRespawnPosition;

        private const float DROWN_DURATION = 0.8f;
        private const float ROTATION_SPEED = 720f;
        private const int DROWN_DAMAGE = 1;

        private Coroutine drownCoroutine;

        public PlayerDrownState(PlayerBrain brain) => this.brain = brain;

        public void SetTargetPositions(Vector3 waterCenter, Vector3 safeRespawnPos)
        {
            this.waterTileCenter = waterCenter;
            this.safeRespawnPosition = safeRespawnPos;
        }

        public void Enter()
        {
            // Lock inputs and stop movement
            if (brain.InputHandler != null)
            {
                brain.InputHandler.DisableInput();
            }

            brain.MovementHandler.Stop();
            brain.transform.position = waterTileCenter;

            drownCoroutine = brain.StartCoroutine(DrownSequenceRoutine());
        }

        public void LogicUpdate()
        {
            // Placeholder rotation animation
            brain.transform.Rotate(Vector3.forward, ROTATION_SPEED * Time.deltaTime);
        }

        public void PhysicsUpdate() { }

        public void Exit()
        {
            // Restore sprite rotation
            brain.transform.rotation = Quaternion.identity;

            // Re-enable inputs
            if (brain.InputHandler != null)
            {
                brain.InputHandler.EnableInput();
            }

            if (drownCoroutine != null)
            {
                brain.StopCoroutine(drownCoroutine);
                drownCoroutine = null;
            }
        }

        private IEnumerator DrownSequenceRoutine()
        {
            yield return new WaitForSeconds(DROWN_DURATION);

            // Relocate to safe ground first
            brain.transform.position = safeRespawnPosition;

            // Apply damage AFTER appearing in safe zone
            if (brain.HealthSystem != null)
            {
                brain.HealthSystem.TakeDamage(DROWN_DAMAGE);
            }

            // Return to Idle or handle death
            if (brain.HealthSystem == null || brain.HealthSystem.CurrentHealth > 0)
            {
                brain.FSM.ChangeState(brain.IdleState);
            }
        }
    }
}