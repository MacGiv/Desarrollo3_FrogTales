namespace FrogGame.Gameplay
{
    using UnityEngine;
    using FrogGame.Core;

    /// <summary>
    /// Handles firing the special stun attack and returning player to idle.
    /// Maneja el disparo del ataque especial de moco y el retorno a Idle.
    /// </summary>
    public class PlayerStunAttackState : IState
    {
        private readonly PlayerBrain brain;

        public PlayerStunAttackState(PlayerBrain brain) => this.brain = brain;

        public void Enter()
        {
            brain.InputHandler.ConsumeStunInput();
            brain.MovementHandler.Stop();

            SpawnStunProjectile();

            brain.FSM.ChangeState(brain.IdleState);
        }

        private void SpawnStunProjectile()
        {
            if (brain.StunPrefab == null)
            {
                Debug.LogWarning("StunPrefab missing on PlayerBrain!");
                return;
            }

            Vector3 spawnPos = brain.ArrowSpawnPoint != null
                ? brain.ArrowSpawnPoint.position
                : brain.transform.position;

            GameObject projObj = Object.Instantiate(brain.StunPrefab, spawnPos, Quaternion.identity);

            if (projObj.TryGetComponent<StunProjectile>(out var projectile))
            {
                projectile.Setup(brain.MovementHandler.FacingDirection);
            }
        }

        public void LogicUpdate() { }
        public void PhysicsUpdate() { }
        public void Exit() { }
    }
}