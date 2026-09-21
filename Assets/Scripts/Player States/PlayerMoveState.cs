using FrogGame.Core;

namespace FrogGame.Gameplay
{
    public class PlayerMoveState : IState
    {
        private readonly PlayerBrain brain;

        public PlayerMoveState(PlayerBrain brain) => this.brain = brain;

        public void Enter() { }

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

            if (brain.InputHandler.MoveInput == UnityEngine.Vector2.zero)
            {
                brain.FSM.ChangeState(brain.IdleState);
            }

            if (brain.InputHandler.StunPressed)
            {
                brain.FSM.ChangeState(brain.StunAttackState);
                return;
            }
        }

        public void PhysicsUpdate()
        {
            brain.MovementHandler.SetVelocity(brain.InputHandler.MoveInput);
        }

        public void Exit() { }
    }
}


