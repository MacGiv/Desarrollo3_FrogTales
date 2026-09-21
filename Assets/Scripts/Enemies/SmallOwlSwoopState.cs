namespace FrogGame.Enemies
{
    using UnityEngine;
    using FrogGame.Core;

    public class SmallOwlSwoopState : IState
    {
        private readonly SmallOwlBrain brain;
        private Vector2 targetPosition;
        private bool isSwooping;
        private float prepareTimer;
        private const float PREPARE_DURATION = 0.4f;

        public SmallOwlSwoopState(SmallOwlBrain brain) => this.brain = brain;

        public void Enter()
        {
            brain.Rb.linearVelocity = Vector2.zero;
            isSwooping = false;
            prepareTimer = 0f;

            // Lock target position at attack initiation
            if (brain.TargetPlayer != null)
            {
                targetPosition = brain.TargetPlayer.position;
            }
            else
            {
                targetPosition = brain.transform.position;
            }
        }

        public void LogicUpdate()
        {
            if (!isSwooping)
            {
                prepareTimer += Time.deltaTime;
                if (prepareTimer >= PREPARE_DURATION)
                {
                    isSwooping = true;
                }
            }
            else
            {
                // Check if swoop reached target destination
                if (Vector2.Distance(brain.transform.position, targetPosition) < 0.3f)
                {
                    brain.FSM.ChangeState(brain.PatrolState);
                }
            }
        }

        public void PhysicsUpdate()
        {
            if (isSwooping)
            {
                Vector2 dir = (targetPosition - (Vector2)brain.transform.position).normalized;
                brain.Rb.linearVelocity = dir * brain.SwoopSpeed;
            }
        }

        public void Exit()
        {
            brain.Rb.linearVelocity = Vector2.zero;
        }
    }
}