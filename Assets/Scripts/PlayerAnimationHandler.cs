namespace FrogGame.Gameplay
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Receives Unity Animation Events and dispatches clean C# actions to listeners.
    /// </summary>
    public class PlayerAnimationHandler : MonoBehaviour
    {
        // C# Events for states/systems to subscribe
        public event Action OnArrowReleased;
        public event Action OnTongueExtended;
        public event Action OnSneezeImpact;

        // Methods called directly by Unity Animation Events

        public void AnimEvent_OnArrowReleased()
        {
            OnArrowReleased?.Invoke();
        }

        public void AnimEvent_OnTongueExtended()
        {
            OnTongueExtended?.Invoke();
        }

        public void AnimEvent_OnSneezeImpact()
        {
            OnSneezeImpact?.Invoke();
        }
    }
}