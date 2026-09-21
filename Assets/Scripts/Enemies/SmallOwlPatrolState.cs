namespace FrogGame.Enemies
{
    using UnityEngine;
    using FrogGame.Core;

    public class SmallOwlPatrolState : IState
    {
        private readonly SmallOwlBrain brain;
        private int currentWaypointIndex = 0;
        private float lastSwoopTime = -999f;

        public SmallOwlPatrolState(SmallOwlBrain brain) => this.brain = brain;

        public void Enter() { }

        public void LogicUpdate()
        {
            // Transition to Swoop if player is in range and cooldown is ready
            if (brain.TargetPlayer != null && Time.time >= lastSwoopTime + brain.AttackCooldown)
            {
                brain.FSM.ChangeState(brain.SwoopState);
                return;
            }

            if (brain.Waypoints == null || brain.Waypoints.Length == 0) return;

            Transform targetWaypoint = brain.Waypoints[currentWaypointIndex];
            if (targetWaypoint == null) return;

            if (Vector2.Distance(brain.transform.position, targetWaypoint.position) < 0.2f)
            {
                currentWaypointIndex = (currentWaypointIndex + 1) % brain.Waypoints.Length;
            }
        }

        public void PhysicsUpdate()
        {
            if (brain.Waypoints == null || brain.Waypoints.Length == 0)
            {
                brain.Rb.linearVelocity = Vector2.zero;
                return;
            }

            Transform targetWaypoint = brain.Waypoints[currentWaypointIndex];
            if (targetWaypoint == null) return;

            Vector2 dir = ((Vector2)targetWaypoint.position - (Vector2)brain.transform.position).normalized;
            brain.Rb.linearVelocity = dir * brain.FlySpeed;
        }

        public void Exit()
        {
            brain.Rb.linearVelocity = Vector2.zero;
            lastSwoopTime = Time.time;
        }
    }
}