namespace FrogGame.Core
{
    using UnityEngine;

    /// <summary>
    /// Finite State Machine controller.
    /// Controlador genérico de la Máquina de Estados.
    /// </summary>
    public class FiniteStateMachine
    {
        public IState CurrentState { get; private set; }

        public void Initialize(IState startingState)
        {
            CurrentState = startingState;
            CurrentState.Enter();
        }

        public void ChangeState(IState newState)
        {
            if (newState == null) return;
            CurrentState?.Exit();
            CurrentState = newState;
            CurrentState.Enter();
        }
    }
}

