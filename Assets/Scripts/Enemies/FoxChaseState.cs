namespace FrogGame.Enemies
{
    using UnityEngine;
    using FrogGame.Core;

    public class FoxChaseState : IState
    {
        private readonly FoxBrain brain;

        public FoxChaseState(FoxBrain brain) => this.brain = brain;

        public void Enter() { }

        public void LogicUpdate()
        {
            // If player lost -> Return to Patrol / Si pierde al jugador, vuelve a patrulla
            if (brain.TargetPlayer == null)
            {
                brain.FSM.ChangeState(brain.PatrolState);
                return;
            }

            float distanceToPlayer = Vector2.Distance(brain.transform.position, brain.TargetPlayer.position);

            // In attack range -> Transition to Attack
            if (distanceToPlayer <= brain.AttackRange)
            {
                brain.FSM.ChangeState(brain.AttackState);
            }
        }

        public void PhysicsUpdate()
        {
            if (brain.TargetPlayer == null) return;

            Vector2 dir = ((Vector2)brain.TargetPlayer.position - (Vector2)brain.transform.position).normalized;
            brain.Rb.linearVelocity = dir * brain.ChaseSpeed;
        }

        public void Exit()
        {
            brain.Rb.linearVelocity = Vector2.zero;
        }
    }
}