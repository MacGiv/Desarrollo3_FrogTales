namespace FrogGame.Enemies
{
    using UnityEngine;
    using FrogGame.Core;

    public class SmallOwlStunnedState : IState
    {
        private readonly SmallOwlBrain brain;
        private float stunDuration;
        private float timer;

        public SmallOwlStunnedState(SmallOwlBrain brain) => this.brain = brain;

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
                brain.FSM.ChangeState(brain.PatrolState);
            }
        }

        public void PhysicsUpdate() { }
        public void Exit() { }
    }
}