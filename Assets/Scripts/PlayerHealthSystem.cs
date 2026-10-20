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
        [SerializeField] private bool isShieldOn = false;

        [Header("Visual Feedback")]
        [SerializeField] private SpriteRenderer playerSprite;

        [SerializeField] private int currentHealth;
        private bool isInvincible = false;
        private Coroutine invincibilityCoroutine;

        public int CurrentHealth => currentHealth;
        public bool IsShieldOn => isShieldOn;
        public int MaxHealth => maxHealth;
        public bool IsInvincible => isInvincible;

        // Events to notify UI or managers without direct coupling
        public static event Action<int, int> OnHealthChanged;
        public static event Action<bool> OnShieldChanged;
        public static event Action OnPlayerDied;

        private void Awake() => currentHealth = maxHealth;

        private void Start()
        {
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            OnShieldChanged?.Invoke(isShieldOn);
        }

        /// <summary>
        /// Decreases player health if not invincible.
        /// Recibe daño si el jugador no está en periodo de invulnerabilidad.
        /// </summary>
        public void TakeDamage(int damageAmount)
        {
            if (isShieldOn)
            {
                isShieldOn = false;
                OnShieldChanged?.Invoke(isShieldOn);
                return;
            }

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
                TriggerInvincibility(invincibilityDuration);
            }
        }

        /// <summary>
        /// Triggers temporary invincibility frames with blinking visual feedback.
        /// Activa cuadros de invulnerabilidad temporales con parpadeo visual.
        /// </summary>
        public void TriggerInvincibility(float duration)
        {
            if (invincibilityCoroutine != null)
            {
                StopCoroutine(invincibilityCoroutine);
            }

            invincibilityCoroutine = StartCoroutine(InvincibilityRoutine(duration));
        }

        /// <summary>
        /// Restores player health up to maxHealth.
        /// </summary>
        public void Heal(int healAmount)
        {
            if (currentHealth <= 0 || currentHealth >= maxHealth) return;

            currentHealth += healAmount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        public void SetShieldState(bool equipped)
        {
            isShieldOn = equipped;
            OnShieldChanged?.Invoke(isShieldOn);
        }

        private IEnumerator InvincibilityRoutine(float duration)
        {
            isInvincible = true;

            float timer = 0f;
            float flashInterval = 0.1f;

            while (timer < duration)
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
            invincibilityCoroutine = null;
        }

        private void Die()
        {
            Debug.Log("[PlayerHealthSystem] The player has died!");
            OnPlayerDied?.Invoke();
        }
    }
}