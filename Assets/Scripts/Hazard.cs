namespace FrogGame.Gameplay
{
    using UnityEngine;
    using FrogGame.Core;

    /// <summary>
    /// Environmental hazard (water, spikes, lava) that damages entities unless invulnerable or grappling.
    /// </summary>
    public class Hazard : MonoBehaviour
    {
        [Header("Hazard Settings")]
        [SerializeField] private int damage = 1;

        [Header("Collision Layers")]
        [SerializeField] private LayerMask playerLayer;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            EvaluateAndApplyDamage(collision);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            EvaluateAndApplyDamage(collision);
        }

        private void EvaluateAndApplyDamage(Collider2D collision)
        {
            int colLayer = 1 << collision.gameObject.layer;

            // Check if it's players layer
            if ((colLayer & playerLayer.value) != 0)
            {
                // Check if player is grappling
                if (collision.TryGetComponent<PlayerBrain>(out var playerBrain))
                {
                    if (playerBrain.FSM.CurrentState == playerBrain.GrappleState)
                    {
                        return; // Ignore collision if player is grappling
                    }
                }

                if (collision.TryGetComponent<IDamageable>(out var damageable))
                {
                    damageable.TakeDamage(damage);
                }
            }
        }
    }
}