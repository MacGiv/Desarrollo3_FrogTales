namespace FrogGame.Gameplay
{
    using UnityEngine;
    using FrogGame.Core;

    public class PlayerGrappleState : IState
    {
        private readonly PlayerBrain brain;
        private Vector2 targetPosition;
        [SerializeField] private float grappleSpeed = 18f;

        public PlayerGrappleState(PlayerBrain brain) => this.brain = brain;

        public void SetTarget(Vector2 target) => targetPosition = target;

        public void Enter()
        {
            brain.MovementHandler.Stop();
        }

        public void LogicUpdate()
        {
            // Move player smoothly towards the grapple point
            // Mover a la rana hacia el punto de enganche
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