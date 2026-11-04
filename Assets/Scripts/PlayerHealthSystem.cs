namespace FrogGame.Gameplay
{
    using System;
    using System.Collections;
    using UnityEngine;
    using FrogGame.Core;

    /// <summary>
    /// Manages the player's health, damage reception, invincibility frames, and healing.
    /// Synchronizes helmet/shield loss with PlayerBrain animator controller swapping.
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

        // Events
        public static event Action<int, int> OnHealthChanged;
        public static event Action<bool> OnShieldChanged;
        public static event Action OnPlayerDied;

        private void Awake()
        {
            currentHealth = maxHealth;
            if (playerSprite == null) playerSprite = GetComponentInChildren<SpriteRenderer>();
        }

        private void Start()
        {
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            OnShieldChanged?.Invoke(isShieldOn);
        }

        /// <summary>
        /// Decreases player health if not invincible. Breaks helmet/shield or triggers HurtState and invincibility frames.
        /// </summary>
        public void TakeDamage(int damageAmount)
        {
            if (isShieldOn)
            {
                isShieldOn = false;
                OnShieldChanged?.Invoke(isShieldOn);

                if (TryGetComponent<PlayerBrain>(out var brainShield))
                {
                    brainShield.SetHelmetEquipped(false);
                }

                TriggerInvincibility(invincibilityDuration, showFlash: true);
                return;
            }

            if (isInvincible || currentHealth <= 0) return;

            currentHealth -= damageAmount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            if (TryGetComponent<PlayerBrain>(out var brain))
            {
                brain.SetHelmetEquipped(false);
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayPlayerDamage(gameObject);
            }

            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            if (currentHealth <= 0)
            {
                Die();
            }
            else
            {
                if (TryGetComponent<PlayerBrain>(out var hurtBrain))
                {
                    hurtBrain.TriggerHurtState();
                }

                TriggerInvincibility(invincibilityDuration, showFlash: true);
            }
        }

        /// <summary>
        /// Triggers temporary invincibility frames. Visual blinking is optional (disabled during room transitions).
        /// </summary>
        public void TriggerInvincibility(float duration, bool showFlash = true)
        {
            if (invincibilityCoroutine != null)
            {
                StopCoroutine(invincibilityCoroutine);
            }

            invincibilityCoroutine = StartCoroutine(InvincibilityRoutine(duration, showFlash));
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

            if (TryGetComponent<PlayerBrain>(out var brain))
            {
                brain.SetHelmetEquipped(equipped);
            }
        }

        private IEnumerator InvincibilityRoutine(float duration, bool showFlash)
        {
            isInvincible = true;

            if (showFlash)
            {
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
            }
            else
            {
                yield return new WaitForSeconds(duration);
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