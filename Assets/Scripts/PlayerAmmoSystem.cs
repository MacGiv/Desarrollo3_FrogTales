namespace FrogGame.Gameplay
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Manages player arrow inventory and ammo consumption.
    /// </summary>
    public class PlayerAmmoSystem : MonoBehaviour
    {
        [Header("Ammo Settings")]
        [SerializeField] private int maxArrows = 25;
        [SerializeField] private int currentArrows = 10;

        public int MaxArrows => maxArrows;
        public int CurrentArrows => currentArrows;

        // Event to notify UI of ammo changes
        public static event Action<int, int> OnArrowCountChanged;

        private void Start()
        {
            // Notify initial ammo state to UI
            OnArrowCountChanged?.Invoke(currentArrows, maxArrows);
        }

        /// <summary>
        /// Decreases arrow count by one if available. Returns true if successful.
        /// </summary>
        public bool ConsumeArrow()
        {
            if (currentArrows <= 0) return false;

            currentArrows--;
            currentArrows = Mathf.Clamp(currentArrows, 0, maxArrows);

            OnArrowCountChanged?.Invoke(currentArrows, maxArrows);
            return true;
        }

        /// <summary>
        /// Adds arrows to player inventory up to maxArrows.
        /// </summary>
        public void AddArrows(int amount)
        {
            if (amount <= 0 || currentArrows >= maxArrows) return;

            currentArrows += amount;
            currentArrows = Mathf.Clamp(currentArrows, 0, maxArrows);

            OnArrowCountChanged?.Invoke(currentArrows, maxArrows);
        }
    }
}