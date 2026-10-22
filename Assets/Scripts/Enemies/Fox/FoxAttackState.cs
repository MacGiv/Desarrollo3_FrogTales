namespace FrogGame.Enemies
{
    using UnityEngine;
    using FrogGame.Core;

    public class FoxAttackState : IState
    {
        private readonly FoxBrain brain;
        private float lastAttackTime;

        public FoxAttackState(FoxBrain brain) => this.brain = brain;

        public void Enter()
        {
            brain.Rb.linearVelocity = Vector2.zero;
            PerformAttack();
        }

        public void LogicUpdate()
        {
            // Cooldown logic before switching back / Esperar cooldown para volver a perseguir o atacar
            if (Time.time >= lastAttackTime + brain.AttackCooldown)
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

        private void PerformAttack()
        {
            lastAttackTime = Time.time;

            if (brain.TargetPlayer != null)
            {
                IDamageable playerDamageable = brain.TargetPlayer.GetComponent<IDamageable>();
                if (playerDamageable != null)
                {
                    playerDamageable.TakeDamage(1);
                }
            }
        }

        public void PhysicsUpdate() { }
        public void Exit() { }
    }
}