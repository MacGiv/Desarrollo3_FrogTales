namespace FrogGame.Gameplay
{
    using System.Collections;
    using UnityEngine;
    using FrogGame.Core;

    /// <summary>
    /// Handles the drowning sequence, damage application post-respawn, input locking, and animation trigger.
    /// </summary>
    public class PlayerDrownState : IState
    {
        private readonly PlayerBrain brain;

        private Vector3 waterTileCenter;
        private Vector3 safeRespawnPosition;

        private const float DROWN_DURATION = 1f;
        private const float RESPAWN_STUN_DURATION = 0.5f;
        private const int DROWN_DAMAGE = 1;

        private Coroutine drownCoroutine;

        public PlayerDrownState(PlayerBrain brain)
        {
            this.brain = brain;
        }

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

            brain.Animator.Play("Drown");
            brain.MovementHandler.Stop();
            brain.transform.position = waterTileCenter;

            drownCoroutine = brain.StartCoroutine(DrownSequenceRoutine());
        }

        public void LogicUpdate() { }

        public void PhysicsUpdate() { }

        public void Exit()
        {
            // Reset transform rotation back to zero as safety precaution
            brain.transform.rotation = Quaternion.identity;

            // Re-enable inputs after exiting state
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

            brain.transform.SetPositionAndRotation(safeRespawnPosition, Quaternion.identity);

            // Apply damage after appearing in safe zone
            if (brain.HealthSystem != null)
            {
                brain.HealthSystem.TakeDamage(DROWN_DAMAGE);
            }

            yield return new WaitForSeconds(RESPAWN_STUN_DURATION);

            // Return to Idle or handle death
            if (brain.HealthSystem == null || brain.HealthSystem.CurrentHealth > 0)
            {
                brain.FSM.ChangeState(brain.IdleState);
            }
        }
    }
}