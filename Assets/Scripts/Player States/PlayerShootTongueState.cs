namespace FrogGame.Gameplay
{
    using UnityEngine;
    using FrogGame.Core;

    public class PlayerShootTongueState : IState
    {
        private readonly PlayerBrain brain;
        private Coroutine tongueCoroutine;

        public PlayerShootTongueState(PlayerBrain brain) => this.brain = brain;

        public void Enter()
        {
            brain.InputHandler.ConsumeTongueInput();
            brain.MovementHandler.Stop();

            // Start tongue coroutine via TongueController or drop carried box
            if (brain.CarryHandler != null && brain.CarryHandler.IsCarrying)
            {
                brain.CarryHandler.TryDropBox();
                brain.FSM.ChangeState(brain.IdleState);
                return; // Redundant just in case
            }
            else
            {
                Vector2 direction = brain.MovementHandler.FacingDirection;
                tongueCoroutine = brain.StartCoroutine(brain.TongueController.ShootTongueRoutine(direction, OnTongueComplete));
            }
        }

        private void OnTongueComplete(TongueController.TongueHitResult result, Vector2 targetPos)
        {
            if (result == TongueController.TongueHitResult.Grapple)
            {
                brain.GrappleState.SetTarget(targetPos);
                brain.FSM.ChangeState(brain.GrappleState);
            }
            else
            {
                brain.FSM.ChangeState(brain.IdleState);
            }
        }

        public void LogicUpdate() { }
        public void PhysicsUpdate() { }

        public void Exit()
        {
            if (tongueCoroutine != null)
            {
                brain.StopCoroutine(tongueCoroutine);
            }
            brain.TongueController.DisableLine();
        }
    }
}
