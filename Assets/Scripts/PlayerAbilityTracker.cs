namespace FrogGame.Gameplay
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Tracks unlocked player capabilities (Tongue, Bow, Sneeze) throughout progression.
    /// </summary>
    public class PlayerAbilityTracker : MonoBehaviour
    {
        [Header("Ability Unlocks")]
        [SerializeField] private bool hasTongue = true;
        [SerializeField] private bool hasBow = false;
        [SerializeField] private bool hasSneeze = false;

        public bool HasTongue => hasTongue;
        public bool HasBow => hasBow;
        public bool HasSneeze => hasSneeze;

        public static event Action<bool> OnBowUnlocked;
        public static event Action<bool> OnSneezeUnlocked;

        public void UnlockBow()
        {
            hasBow = true;
            OnBowUnlocked?.Invoke(hasBow);
        }

        public void UnlockSneeze()
        {
            hasSneeze = true;
            OnSneezeUnlocked?.Invoke(hasSneeze);
        }
    }
}