namespace FrogGame.Gameplay
{
    using UnityEngine;

    /// <summary>
    /// Health pickup item dropped by enemies or found in chests.
    /// Ítem de vida que sueltan los enemigos o se encuentra en mapas.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class PickupHealth : MonoBehaviour
    {
        [SerializeField] private int healAmount = 1;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent<PlayerHealthSystem>(out var healthSystem))
            {
                if (healthSystem.CurrentHealth < healthSystem.MaxHealth)
                {
                    healthSystem.Heal(healAmount);
                    Destroy(gameObject);
                }
            }
        }
    }
}