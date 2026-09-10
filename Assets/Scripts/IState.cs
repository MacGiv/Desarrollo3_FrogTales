namespace FrogGame.Core
{
    /// <summary>
    /// State interface for FSM architecture.
    /// Interfaz base para los estados de la máquina de estados.
    /// </summary>
    public interface IState
    {
        void Enter();
        void LogicUpdate();
        void PhysicsUpdate();
        void Exit();
    }
}

