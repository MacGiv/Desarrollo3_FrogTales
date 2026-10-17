namespace FrogGame.Gameplay
{
    using FrogGame.Core;

    /// <summary>
    /// Handles player movement logic, carrying interactions, and state transitions.
    /// Maneja la lógica de movimiento, interacciones al cargar objetos y transiciones de estado.
    /// </summary>
    public class PlayerMoveState : IState
    {
        private readonly PlayerBrain brain;

        public PlayerMoveState(PlayerBrain brain)
        {
            this.brain = brain;
        }

        public void Enter()
        {
            brain.Animator.Play("Locomotion");
        }

        public void LogicUpdate()
        {
            // Block attacks while carrying an object / Drop object on action input
            if (brain.CarryHandler != null && brain.CarryHandler.IsCarrying)
            {
                if (brain.InputHandler.TonguePressed)
                {
                    brain.InputHandler.ConsumeTongueInput();
                    brain.CarryHandler.TryDropBox();
                    return;
                }

                if (brain.InputHandler.ArrowPressed)
                {
                    brain.InputHandler.ConsumeArrowInput();
                    return;
                }

                if (brain.InputHandler.StunPressed)
                {
                    brain.InputHandler.ConsumeStunInput();
                    return;
                }
            }
            else
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

                if (brain.InputHandler.StunPressed)
                {
                    brain.FSM.ChangeState(brain.StunAttackState);
                    return;
                }
            }

            if (brain.InputHandler.MoveInput == UnityEngine.Vector2.zero)
            {
                brain.FSM.ChangeState(brain.IdleState);
            }
        }

        public void PhysicsUpdate()
        {
            brain.MovementHandler.SetVelocity(brain.InputHandler.MoveInput);
        }

        public void Exit() { }
    }
}