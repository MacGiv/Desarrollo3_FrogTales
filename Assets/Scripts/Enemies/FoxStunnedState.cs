namespace FrogGame.Enemies
{
    using UnityEngine;
    using FrogGame.Core;

    /// <summary>
    /// State when the enemy is stunned and recovering from knockback momentum.
    /// </summary>
    public class FoxStunnedState : IState
    {
        private readonly FoxBrain brain;
        private float stunDuration;
        private float timer;
        private const float KNOCKBACK_DECELERATION = 25f;

        public FoxStunnedState(FoxBrain brain) => this.brain = brain;

        public void SetStunDuration(float duration) => stunDuration = duration;

        public void Enter()
        {
            timer = 0f;
        }

        public void LogicUpdate()
        {
            timer += Time.deltaTime;

            if (timer >= stunDuration)
            {
                if (brain.TargetPlayer != null)
                {
                    brain.FSM.ChangeState(brain.ChaseState);
                }
                else
                {
                    brain.FSM.ChangeState(brain.PatrolState);
                }
            }
        }

        public void PhysicsUpdate()
        {
            // Smoothly reduce knockback velocity to zero during stun
            if (brain.Rb.linearVelocity != Vector2.zero)
            {
                brain.Rb.linearVelocity = Vector2.MoveTowards(
                    brain.Rb.linearVelocity,
                    Vector2.zero,
                    KNOCKBACK_DECELERATION * Time.fixedDeltaTime
                );
            }
        }

        public void Exit()
        {
            brain.Rb.linearVelocity = Vector2.zero;
        }
    }
}