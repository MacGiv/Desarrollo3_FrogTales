namespace FrogGame.Gameplay
{
    using UnityEngine;
    using FrogGame.Core;

    /// <summary>
    /// Collectible arrow dropped on impact. Restores ammo when touched or pulled with tongue.
    /// Objeto recolectable de flecha. Restaura munición al tocarse o atraparse con la lengua.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class PickupArrow : MonoBehaviour, IPickupable
    {
        [Header("Pickup Settings")]
        [SerializeField] private int ammoAmount = 1;

        public bool Collect(GameObject collector)
        {
            // Busca el sistema de munición directamente o a través de PlayerBrain
            if (collector.TryGetComponent<PlayerBrain>(out var brain) && brain.AmmoSystem != null)
            {
                brain.AmmoSystem.AddArrows(ammoAmount);
                Destroy(gameObject);
                return true;
            }
            else if (collector.TryGetComponent<PlayerAmmoSystem>(out var ammoSystem))
            {
                ammoSystem.AddArrows(ammoAmount);
                Destroy(gameObject);
                return true;
            }

            return false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Collect(collision.gameObject);
        }
    }
}