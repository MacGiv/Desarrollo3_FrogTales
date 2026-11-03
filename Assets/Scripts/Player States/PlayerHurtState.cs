namespace FrogGame.Gameplay
{
    using UnityEngine;
    using FrogGame.Core;

    /// <summary>
    /// State triggered when the player takes damage. Disables input, displays directional hurt sprite (1 frame per direction), and returns to Idle upon completion.
    /// </summary>
    public class PlayerHurtState : IState
    {
        private readonly PlayerBrain brain;
        private readonly float hurtDuration;
        private float timer;

        public PlayerHurtState(PlayerBrain brain, float hurtDuration = 0.3f)
        {
            this.brain = brain;
            this.hurtDuration = hurtDuration;
        }

        public void Enter()
        {
            timer = 0f;

            brain.MovementHandler?.Stop();
            brain.InputHandler?.DisableInput();

            if (brain.Animator != null && brain.MovementHandler != null)
            {
                Vector2 facingDir = brain.MovementHandler.FacingDirection;
                brain.Animator.SetFloat("MoveX", facingDir.x);
                brain.Animator.SetFloat("MoveY", facingDir.y);

                brain.Animator.Play("Hurt");
            }
        }

        public void LogicUpdate()
        {
            timer += Time.deltaTime;

            if (timer >= hurtDuration)
            {
                brain.FSM.ChangeState(brain.IdleState);
            }
        }

        public void PhysicsUpdate() { }

        public void Exit()
        {
            brain.InputHandler?.EnableInput();
        }
    }
}