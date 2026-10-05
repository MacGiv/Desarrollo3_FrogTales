namespace FrogGame.Gameplay
{
    using System;
    using System.Collections;
    using UnityEngine;
    using FrogGame.Core;

    /// <summary>
    /// Manages the player's health, damage reception, invincibility frames, and healing.
    /// Gestiona la vida del jugador, recepción de daño, cuadros de invencibilidad y curación.
    /// </summary>
    public class PlayerHealthSystem : MonoBehaviour, IDamageable
    {
        [Header("Health Settings")]
        [SerializeField] private int maxHealth = 3;
        [SerializeField] private float invincibilityDuration = 1.0f;

        [Header("Visual Feedback")]
        [SerializeField] private SpriteRenderer playerSprite;

        [SerializeField] private int currentHealth;
        private bool isInvincible = false;

        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;

        // Events to notify UI or managers without direct coupling
        // Eventos para notificar a la UI o Managers sin acoplamiento directo
        public static event Action<int, int> OnHealthChanged; // (currentHealth, maxHealth)
        public static event Action OnPlayerDied;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        private void Start()
        {
            // Notify initial health state to UI / Notificar estado inicial a la UI
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        /// <summary>
        /// Decreases player health if not invincible.
        /// Recibe daño si el jugador no está en periodo de invulnerabilidad.
        /// </summary>
        public void TakeDamage(int damageAmount)
        {
            if (isInvincible || currentHealth <= 0) return;

            currentHealth -= damageAmount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            if (currentHealth <= 0)
            {
                Die();
            }
            else
            {
                StartCoroutine(InvincibilityRoutine());
            }
        }

        /// <summary>
        /// Restores player health up to maxHealth.
        /// Restaura vida al jugador hasta el máximo permitido.
        /// </summary>
        public void Heal(int healAmount)
        {
            if (currentHealth <= 0 || currentHealth >= maxHealth) return;

            currentHealth += healAmount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        /// <summary>
        /// Handles temporized invincibility and blinking sprite visual effect.
        /// Maneja el tiempo de invulnerabilidad y el parpadeo del sprite.
        /// </summary>
        private IEnumerator InvincibilityRoutine()
        {
            isInvincible = true;

            float timer = 0f;
            float flashInterval = 0.1f;

            while (timer < invincibilityDuration)
            {
                if (playerSprite != null)
                {
                    playerSprite.enabled = !playerSprite.enabled;
                }

                yield return new WaitForSeconds(flashInterval);
                timer += flashInterval;
            }

            if (playerSprite != null)
            {
                playerSprite.enabled = true;
            }

            isInvincible = false;
        }

        private void Die()
        {
            Debug.Log("[PlayerHealthSystem] The player has died!");
            OnPlayerDied?.Invoke();
            // TODO: Cambiar estado a PlayerDeathState
        }
    }
}
