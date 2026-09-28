namespace FrogGame.Gameplay
{
    using FrogGame.Core;
    using UnityEngine;

    public class Hazard : MonoBehaviour
    {
        [Header("Hazard Settings")]
        [SerializeField] private int damage = 1;

        [Header("Collision Layers")]
        [SerializeField] private LayerMask playerLayer;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            int colLayer = 1 << collision.gameObject.layer;

            // 1. Check if hit player
            if ((colLayer & playerLayer) != 0)
            {
                // 2. Get the component
                IDamageable damageable = collision.GetComponent<IDamageable>();

                // 3. If not null, apply the damage.
                if (damageable != null)
                {
                    damageable.TakeDamage(damage);
                }
            }
        }
    }
}