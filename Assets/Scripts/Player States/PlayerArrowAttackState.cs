namespace FrogGame.Gameplay
{
    using UnityEngine;
    using FrogGame.Core;

    /// <summary>
    /// Handles spawning arrows and returning player to idle state.
    /// Maneja el disparo de flechas y el retorno del jugador al estado de reposo.
    /// </summary>
    public class PlayerArrowAttackState : IState
    {
        private readonly PlayerBrain brain;

        public PlayerArrowAttackState(PlayerBrain brain) => this.brain = brain;

        public void Enter()
        {
            brain.InputHandler.ConsumeArrowInput();
            brain.MovementHandler.Stop();

            // Spawn and launch arrow / Instanciar y disparar la flecha
            SpawnArrow();

            // Return to Idle immediately (or can be delayed via animation/timer)
            // Retorno a Idle tras disparar
            brain.FSM.ChangeState(brain.IdleState);
        }

        private void SpawnArrow()
        {
            if (brain.ArrowPrefab == null)
            {
                Debug.LogWarning("ArrowPrefab missing on PlayerBrain!");
                return;
            }

            Vector3 spawnPos = brain.arrowSpawnPoint != null ? brain.arrowSpawnPoint.position : brain.transform.position;

            GameObject arrowObj = Object.Instantiate(brain.ArrowPrefab, spawnPos, Quaternion.identity);

            if (arrowObj.TryGetComponent<Arrow>(out var arrow))
            {
                arrow.Setup(brain.MovementHandler.FacingDirection);
            }
        }

        public void LogicUpdate() { }
        public void PhysicsUpdate() { }
        public void Exit() { }
    }
}