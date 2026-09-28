namespace FrogGame.Gameplay
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Events;
    using FrogGame.Core;

    [RequireComponent(typeof(Collider2D))]
    public class GrabbableItem : MonoBehaviour
    {
        public enum ItemType
        {
            Collectible,
            Shield,
            HeavyBlock,
            CarriableBox // Nueva opción: caja que la rana levanta y lleva encima
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
        public Collider2D ItemCollider => itemCollider;

        private void Awake()
        {
            itemCollider = GetComponent<Collider2D>();
        }

        public void Grab(Transform pullTarget)
        {
            if (isBeingPulled) return;

            isBeingPulled = true;
            onGrabbed?.Invoke();

            StartCoroutine(PullRoutine(pullTarget));
        }

        private IEnumerator PullRoutine(Transform pullTarget)
        {
            if (itemCollider != null && type != ItemType.HeavyBlock)
            {
                itemCollider.enabled = false;
            }

            if (transform.parent != null)
            {
                transform.SetParent(null);
            }

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
                case ItemType.Shield:
                    if (gameObject != null) Destroy(gameObject);
                    break;

                case ItemType.HeavyBlock:
                    if (itemCollider != null) itemCollider.enabled = true;
                    isBeingPulled = false;
                    break;

                case ItemType.CarriableBox:
                    // Notificar al PlayerCarryHandler que la caja llegó a la rana
                    if (pullTarget != null && pullTarget.TryGetComponent<PlayerCarryHandler>(out var carryHandler))
                    {
                        carryHandler.AttachBox(this);
                    }
                    isBeingPulled = false;
                    break;
            }
        }
    }
}
