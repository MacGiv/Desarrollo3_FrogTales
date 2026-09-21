namespace FrogGame.Core
{
    /// <summary>
    /// Interface for entities or objects that can be stunned by the frog's special attack.
    /// Interfaz para entidades u objetos que pueden ser aturdidos por el ataque de moco.
    /// </summary>
    public interface IStunnable
    {
        void ApplyStun(float duration);
    }
}