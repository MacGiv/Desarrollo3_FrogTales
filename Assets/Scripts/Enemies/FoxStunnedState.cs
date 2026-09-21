namespace FrogGame.Enemies
{
    using UnityEngine;
    using FrogGame.Core;

    /// <summary>
    /// State when the enemy is stunned by the frog's booger attack.
    /// Estado cuando el enemigo queda aturdido por el ataque de moco.
    /// </summary>
    public class FoxStunnedState : IState
    {
        private readonly FoxBrain brain;
        private float stunDuration;
        private float timer;

        public FoxStunnedState(FoxBrain brain) => this.brain = brain;

        public void SetStunDuration(float duration) => stunDuration = duration;

        public void Enter()
        {
            timer = 0f;
            brain.Rb.linearVelocity = Vector2.zero;
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

        public void PhysicsUpdate() { }
        public void Exit() { }
    }
}