namespace FrogGame.Gameplay
{
    using UnityEngine;

    /// <summary>
    /// Handles shopkeeper sleeping visuals and separate Zzz animation effect.
    /// Maneja los visuales del NPC durmiendo y el efecto animado Zzz por separado.
    /// </summary>
    public class SleepingNPC : MonoBehaviour
    {
        [Header("Animation References")]
        [SerializeField] private Animator npcAnimator;
        [SerializeField] private GameObject zzzVisualEffect;

        [Header("State Settings")]
        [SerializeField] private string sleepingStateName = "NPC_Sleeping";

        private void Start()
        {
            SetSleepingState(true);
        }

        /// <summary>
        /// Enables or disables the sleeping animation state and Zzz visual effect.
        /// </summary>
        public void SetSleepingState(bool isSleeping)
        {
            if (npcAnimator != null && !string.IsNullOrEmpty(sleepingStateName))
            {
                npcAnimator.Play(sleepingStateName);
            }

            if (zzzVisualEffect != null)
            {
                zzzVisualEffect.SetActive(isSleeping);
            }
        }
    }
}