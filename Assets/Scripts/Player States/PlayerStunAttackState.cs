namespace FrogGame.Gameplay
{
    using System.Collections;
    using UnityEngine;
    using FrogGame.Core;

    /// <summary>
    /// Executes sneeze attack on frame event, reuses ShootTongue animation, and returns to Idle via dynamic duration.
    /// </summary>
    public class PlayerStunAttackState : IState
    {
        private readonly PlayerBrain brain;
        private Coroutine sneezeCoroutine;
        private bool hasExecutedSneeze;

        public PlayerStunAttackState(PlayerBrain brain) => this.brain = brain;

        public void Enter()
        {
            brain.InputHandler.ConsumeStunInput();
            brain.MovementHandler.Stop();

            hasExecutedSneeze = false;

            if (brain.AnimationHandler != null)
            {
                brain.AnimationHandler.OnSneezeImpact += HandleSneezeImpact;
            }

            // Reuses ShootTongue animation
            brain.Animator.Play("Sneeze");
            sneezeCoroutine = brain.StartCoroutine(PlayAnimationAndReturnToIdle());
        }

        private void HandleSneezeImpact()
        {
            if (hasExecutedSneeze) return;
            hasExecutedSneeze = true;
            ExecuteSneeze();
        }

        private IEnumerator PlayAnimationAndReturnToIdle()
        {
            yield return null; // Wait 1 frame for Animator state update

            float animLength = brain.Animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(animLength);

            // Fallback safety
            if (!hasExecutedSneeze)
            {
                ExecuteSneeze();
            }

            brain.FSM.ChangeState(brain.IdleState);
        }

        private void ExecuteSneeze()
        {
            if (brain.SneezeHandler != null)
            {
                brain.SneezeHandler.ExecuteSneeze();
            }
            else
            {
                Debug.LogWarning("[PlayerStunAttackState] PlayerSneezeHandler reference missing on PlayerBrain!");
            }
        }

        public void LogicUpdate() { }
        public void PhysicsUpdate() { }

        public void Exit()
        {
            if (brain.AnimationHandler != null)
            {
                brain.AnimationHandler.OnSneezeImpact -= HandleSneezeImpact;
            }

            if (sneezeCoroutine != null)
            {
                brain.StopCoroutine(sneezeCoroutine);
                sneezeCoroutine = null;
            }
        }
    }
}