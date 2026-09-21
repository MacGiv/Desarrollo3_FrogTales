using FrogGame.Core;

namespace FrogGame.Gameplay
{
    public class PlayerIdleState : IState
    {
        private readonly PlayerBrain brain;

        public PlayerIdleState(PlayerBrain brain) => this.brain = brain;

        public void Enter() => brain.MovementHandler.Stop();

        public void LogicUpdate()
        {
            if (brain.InputHandler.TonguePressed)
            {
                brain.FSM.ChangeState(brain.TongueState);
                return;
            }

            if (brain.InputHandler.ArrowPressed)
            {
                brain.FSM.ChangeState(brain.ArrowState);
                return;
            }

            if (brain.InputHandler.MoveInput != UnityEngine.Vector2.zero)
            {
                brain.FSM.ChangeState(brain.MoveState);
            }

            if (brain.InputHandler.StunPressed)
            {
                brain.FSM.ChangeState(brain.StunAttackState);
                return;
            }
        }

        public void PhysicsUpdate() { }
        public void Exit() { }
    }
}

