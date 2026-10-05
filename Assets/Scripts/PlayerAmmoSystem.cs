namespace FrogGame.Gameplay
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Manages player arrow inventory, ammo consumption, and cooldown.
    /// </summary>
    public class PlayerAmmoSystem : MonoBehaviour
    {
        [Header("Ammo Settings")]
        [SerializeField] private int currentArrows = 10;
        [SerializeField] private int maxArrows = 25;

        [Header("Cooldown Settings")]
        [SerializeField] private float shootCooldown = 0.5f;
        [SerializeField] private SpriteRenderer readyIndicator; // Indicador visual opcional (punto u orbe sobre el personaje)

        private float lastShootTime = -999f;

        public int CurrentArrows => currentArrows;
        public bool CanShoot => currentArrows > 0 && Time.time >= lastShootTime + shootCooldown;

        public static event Action<int> OnArrowCountChanged;

        private void Start()
        {
            OnArrowCountChanged?.Invoke(currentArrows);
            UpdateIndicator();
        }

        private void Update()
        {
            UpdateIndicator();
        }

        /// <summary>
        /// Tries to consume one arrow considering cooldown.
        /// </summary>
        public bool TryConsumeArrow()
        {
            if (!CanShoot) return false;

            currentArrows--;
            lastShootTime = Time.time;
            OnArrowCountChanged?.Invoke(currentArrows);
            UpdateIndicator();

            return true;
        }

        public void AddArrows(int amount)
        {
            if (amount <= 0 || currentArrows >= maxArrows) return;

            currentArrows += amount;
            OnArrowCountChanged?.Invoke(currentArrows);
            UpdateIndicator();
        }

        private void UpdateIndicator()
        {
            if (readyIndicator == null) return;

            if (CanShoot)
            {
                readyIndicator.color = Color.green; // Flecha lista
            }
            else
            {
                readyIndicator.color = (currentArrows <= 0) ? Color.red : new Color(1f, 1f, 1f, 0.2f); // Cooldown o sin balas
            }
        }
    }
}