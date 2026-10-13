namespace FrogGame.UI
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using FrogGame.Gameplay;

    /// <summary>
    /// Displays health and arrow ammo updates on the HUD UI.
    /// </summary>
    public class PlayerHUD : MonoBehaviour
    {
        [Header("Health UI References")]
        [SerializeField] private Slider healthSlider;

        [Header("Shield UI Refeences")]
        [SerializeField] private Image shieldIcon;

        [Header("Arrow UI")]
        [SerializeField] private TextMeshProUGUI arrowText;

        private void OnEnable()
        {
            PlayerHealthSystem.OnHealthChanged += UpdateHealthUI;
            PlayerHealthSystem.OnShieldChanged += UpdateShieldUI;
            PlayerAmmoSystem.OnArrowCountChanged += UpdateArrowUI;
        }

        private void OnDisable()
        {
            PlayerHealthSystem.OnHealthChanged -= UpdateHealthUI;
            PlayerHealthSystem.OnShieldChanged -= UpdateShieldUI;
            PlayerAmmoSystem.OnArrowCountChanged -= UpdateArrowUI;
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

        /// <summary>
        /// Toggles the visibility of the helmet icon on the HUD based on equipped state.
        /// </summary>
        private void UpdateShieldUI(bool isShieldOn)
        {
            if (shieldIcon != null)
            {
                shieldIcon.enabled = isShieldOn;
            }
        }

        /// <summary>
        /// Updates the arrow text counter.
        /// </summary>
        private void UpdateArrowUI(int currentArrows)
        {
            if (arrowText != null)
            {
                arrowText.text = currentArrows.ToString();
            }
        }
    }
}