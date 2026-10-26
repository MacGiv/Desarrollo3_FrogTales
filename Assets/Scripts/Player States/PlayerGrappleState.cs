namespace FrogGame.Gameplay
{
    using UnityEngine;
    using FrogGame.Core;

    public class PlayerGrappleState : IState
    {
        [SerializeField] private float grappleSpeed = 18f;

        private readonly PlayerBrain brain;
        private Vector2 targetPosition;

        public PlayerGrappleState(PlayerBrain brain) => this.brain = brain;

        public void SetTarget(Vector2 target) => targetPosition = target;

        public void Enter()
        {
            brain.MovementHandler.Stop();
            brain.Animator.Play("Grapple");
        }

        public void LogicUpdate()
        {
            if (brain.GrappleCollisionDetector != null && !brain.GrappleCollisionDetector.CanGrapple())
            {
                brain.FSM.ChangeState(brain.IdleState);

                return;
            }

            Vector2 currentPos = brain.transform.position;
            brain.transform.position = Vector2.MoveTowards(currentPos, targetPosition, grappleSpeed * Time.deltaTime);

            if (Vector2.Distance(brain.transform.position, targetPosition) < 0.3f)
            {
                brain.FSM.ChangeState(brain.IdleState);
            }
        }

        public void PhysicsUpdate() { }

        public void Exit() { }
    }
}