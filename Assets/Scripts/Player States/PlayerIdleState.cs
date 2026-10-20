namespace FrogGame.Gameplay
{
    using FrogGame.Core;

    /// <summary>
    /// Handles player idle state, carrying restrictions, and attack blocking.
    /// Maneja el estado de reposo, restricciones al cargar objetos y bloqueo de ataques.
    /// </summary>
    public class PlayerIdleState : IState
    {
        private readonly PlayerBrain brain;

        public PlayerIdleState(PlayerBrain brain) => this.brain = brain;

        public void Enter()
        {
            brain.MovementHandler.Stop();
            brain.Animator.Play("Idle");
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

            // 4. Locomotion transition
            if (brain.InputHandler.MoveInput != UnityEngine.Vector2.zero)
            {
                brain.FSM.ChangeState(brain.MoveState);
            }
        }

        public void PhysicsUpdate() { }
        public void Exit() { }
    }
}