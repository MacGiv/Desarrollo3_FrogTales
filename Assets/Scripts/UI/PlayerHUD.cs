namespace FrogGame.UI
{
    using UnityEngine;
    using UnityEngine.UI;
    using FrogGame.Gameplay;

    /// <summary>
    /// Displays health updates on the HUD UI.
    /// </summary>
    public class PlayerHUD : MonoBehaviour
    {
        [Header("Health UI References")]
        [SerializeField] private Slider healthSlider;

        private void OnEnable()
        {
            PlayerHealthSystem.OnHealthChanged += UpdateHealthUI;
        }

        private void OnDisable()
        {
            PlayerHealthSystem.OnHealthChanged -= UpdateHealthUI;
        }

        /// <summary>
        /// Updates health slider bounds and current value.
        /// </summary>
        private void UpdateHealthUI(int currentHealth, int maxHealth)
        {
            if (healthSlider != null)
            {
                healthSlider.maxValue = maxHealth;
                healthSlider.value = currentHealth;
            }
        }
    }
}