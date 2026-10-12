namespace FrogGame.Gameplay
{
    using System.Collections;
    using UnityEngine;
    using FrogGame.Core;

    /// <summary>
    /// Handles spawning arrows via frame-precise animation events and returning to idle via dynamic clip length.
    /// </summary>
    public class PlayerArrowAttackState : IState
    {
        private readonly PlayerBrain brain;
        private Coroutine attackCoroutine;
        private bool hasSpawnedArrow;

        public PlayerArrowAttackState(PlayerBrain brain) => this.brain = brain;

        public void Enter()
        {
            brain.InputHandler.ConsumeArrowInput();
            brain.MovementHandler.Stop();

            if (brain.AmmoSystem != null && brain.AmmoSystem.TryConsumeArrow())
            {
                hasSpawnedArrow = false;

                // Subscribe to C# Event from AnimationHandler
                if (brain.AnimationHandler != null)
                {
                    brain.AnimationHandler.OnArrowReleased += HandleArrowReleased;
                }

                brain.Animator.Play("ShootArrow");
                attackCoroutine = brain.StartCoroutine(PlayAnimationAndReturnToIdle());
            }
            else
            {
                brain.FSM.ChangeState(brain.IdleState);
            }
        }

        private void HandleArrowReleased()
        {
            if (hasSpawnedArrow) return;
            hasSpawnedArrow = true;
            SpawnArrow();
        }

        private IEnumerator PlayAnimationAndReturnToIdle()
        {
            yield return null; // Wait 1 frame for Animator state update

            float animLength = brain.Animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(animLength);

            // Fallback safety if event was missing on the current clip
            if (!hasSpawnedArrow)
            {
                SpawnArrow();
            }

            brain.FSM.ChangeState(brain.IdleState);
        }

        private void SpawnArrow()
        {
            if (brain.ArrowPrefab == null) return;

            Vector3 spawnPos = brain.ArrowSpawnPoint != null ? brain.ArrowSpawnPoint.position : brain.transform.position;
            GameObject arrowObj = Object.Instantiate(brain.ArrowPrefab, spawnPos, Quaternion.identity);

            if (arrowObj.TryGetComponent<Arrow>(out var arrow))
            {
                arrow.Setup(brain.MovementHandler.FacingDirection);
            }
        }

        public void LogicUpdate() { }
        public void PhysicsUpdate() { }

        public void Exit()
        {
            if (brain.AnimationHandler != null)
            {
                brain.AnimationHandler.OnArrowReleased -= HandleArrowReleased;
            }

            if (attackCoroutine != null)
            {
                brain.StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }
        }
    }
}