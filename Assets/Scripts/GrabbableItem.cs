namespace FrogGame.Gameplay
{
    using System.Collections;
    using FrogGame.Core;
    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// Component for items or objects that can be pulled or grabbed by the frog's tongue.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class GrabbableItem : MonoBehaviour
    {
        public enum ItemType
        {
            Collectible, // Flies, keys, health
            Shield,      // Enemy shields
            HeavyBlock   // Box or blocks that can be attracted to player
        }

        [Header("Item Settings")]
        [SerializeField] private ItemType type = ItemType.Collectible;
        [SerializeField] private float pullSpeed = 15f;
        [SerializeField] private float stopDistance = 0.5f;

        [Header("Events")]
        [SerializeField] private UnityEvent onGrabbed;
        [SerializeField] private UnityEvent onCollected;

        private bool isBeingPulled = false;
        private Collider2D itemCollider;

        public ItemType Type => type;
        public bool IsBeingPulled => isBeingPulled;

        private void Awake()
        {
            itemCollider = GetComponent<Collider2D>();
        }

        /// <summary>
        /// Called when the tongue impacts this object.
        /// </summary>
        public void Grab(Transform pullTarget)
        {
            if (isBeingPulled) return;

            isBeingPulled = true;
            onGrabbed?.Invoke();

            StartCoroutine(PullRoutine(pullTarget));
        }

        private IEnumerator PullRoutine(Transform pullTarget)
        {
            // Deactivate collider to avoid erratic behaviours
            if (itemCollider != null && type != ItemType.HeavyBlock)
            {
                itemCollider.enabled = false;
            }

            // If the item has a parent, decouple from enemy
            if (transform.parent != null)
            {
                transform.SetParent(null);
            }

            // Object's movement to frog
            while (pullTarget != null && Vector2.Distance(transform.position, pullTarget.position) > stopDistance)
            {
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    pullTarget.position,
                    pullSpeed * Time.deltaTime
                );
                yield return null;
            }

            onCollected?.Invoke();

            if (pullTarget != null && TryGetComponent<IPickupable>(out var pickupable))
            {
                pickupable.Collect(pullTarget.gameObject);
            }

            switch (type)
            {
                case ItemType.Collectible:
                    if (gameObject != null)
                    {
                        Destroy(gameObject);
                    }
                    break;

                case ItemType.Shield:
                    Destroy(gameObject);
                    break;

                case ItemType.HeavyBlock:
                    if (itemCollider != null) itemCollider.enabled = true;
                    isBeingPulled = false;
                    break;
            }
        }
    }
}
