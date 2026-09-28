namespace FrogGame.Gameplay
{
    using UnityEngine;
    using FrogGame.Core;

    /// <summary>
    /// Health pickup item that restores health when touched or collected by tongue.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class PickupHealth : MonoBehaviour, IPickupable
    {
        [SerializeField] private int healAmount = 1;

        public bool Collect(GameObject collector)
        {
            if (collector.TryGetComponent<PlayerHealthSystem>(out var healthSystem))
            {
                if (healthSystem.CurrentHealth < healthSystem.MaxHealth)
                {
                    healthSystem.Heal(healAmount);
                    Destroy(gameObject);
                    return true;
                }
            }
            return false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Collect(collision.gameObject);
        }
    }
}
