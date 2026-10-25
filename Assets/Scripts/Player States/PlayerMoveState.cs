namespace FrogGame.Gameplay
{
    using FrogGame.Core;

    /// <summary>
    /// Handles player locomotion, carrying restrictions, and attack blocking during movement.
    /// Maneja la locomoción del jugador, restricciones al cargar objetos y bloqueo de ataques durante el movimiento.
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
            brain.Animator.Play("Move");
        }

        public void LogicUpdate()
        {
            bool isCarrying = brain.CarryHandler != null && brain.CarryHandler.IsCarrying;

            // 1. Tongue input: Drops carried box/stone via TongueState or triggers normal tongue shoot
            if (brain.InputHandler.TonguePressed)
            {
                brain.FSM.ChangeState(brain.TongueState);
                return;
            }

            // 2. Arrow input: Blocked if carrying an object
            if (brain.InputHandler.ArrowPressed)
            {
                if (isCarrying)
                {
                    brain.InputHandler.ConsumeArrowInput();
                }
                else
                {
                    brain.FSM.ChangeState(brain.ArrowState);
                    return;
                }
            }

            // 3. Stun input: Blocked if carrying an object
            if (brain.InputHandler.StunPressed)
            {
                if (isCarrying)
                {
                    brain.InputHandler.ConsumeStunInput();
                }
                else
                {
                    brain.FSM.ChangeState(brain.StunAttackState);
                    return;
                }
            }

            // 4. Return to Idle transition
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