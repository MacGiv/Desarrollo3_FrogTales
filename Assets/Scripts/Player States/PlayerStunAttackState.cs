namespace FrogGame.Gameplay
{
    using UnityEngine;
    using FrogGame.Core;

    /// <summary>
    /// Handles executing the sneeze attack and returning the player state machine back to Idle.
    /// </summary>
    public class PlayerStunAttackState : IState
    {
        private readonly PlayerBrain brain;

        public PlayerStunAttackState(PlayerBrain brain) => this.brain = brain;

        public void Enter()
        {
            // Consume button input to prevent duplicate triggers
            brain.InputHandler.ConsumeStunInput();

            // Stop player movement during attack execution
            brain.MovementHandler.Stop();

            // Execute the area-of-effect sneeze attack
            if (brain.SneezeHandler != null)
            {
                brain.SneezeHandler.ExecuteSneeze();
            }
            else
            {
                Debug.LogWarning("[PlayerStunAttackState] PlayerSneezeHandler reference missing on PlayerBrain!");
            }

            // Immediately transition back to Idle state
            brain.FSM.ChangeState(brain.IdleState);
        }

        public void LogicUpdate() { }
        public void PhysicsUpdate() { }
        public void Exit() { }
    }
}