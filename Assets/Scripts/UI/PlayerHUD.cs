namespace FrogGame.UI
{
    using TMPro;
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

        [Header("Arrow UI")]
        [SerializeField] private TextMeshProUGUI arrowText;

        private void OnEnable()
        {
            PlayerHealthSystem.OnHealthChanged += UpdateHealthUI;
            PlayerAmmoSystem.OnArrowCountChanged += UpdateArrowUI;
        }

        private void OnDisable()
        {
            PlayerHealthSystem.OnHealthChanged -= UpdateHealthUI;
            PlayerAmmoSystem.OnArrowCountChanged += UpdateArrowUI;
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
        /// Updates the arrow text counter.
        /// </summary>
        private void UpdateArrowUI(int currentArrows, int maxArrows)
        {
            if (arrowText != null)
            {
                arrowText.text = currentArrows.ToString();
            }
        }
    }
}