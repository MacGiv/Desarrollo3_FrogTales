namespace FrogGame.Gameplay
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// Component for items or objects that can be pulled or grabbed by the frog's tongue.
    /// Componente para ítems u objetos que pueden ser atraídos o agarrados por la lengua de la rana.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class GrabbableItem : MonoBehaviour
    {
        public enum ItemType
        {
            Collectible, // Flies, keys, health (trae al jugador y se destruye/recoge)
            Shield,      // Escudo robado a un enemigo (lo desengancha y destruye/cae)
            HeavyBlock   // Cajas o bloques arrastrales (se mueven hacia la rana)
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
        /// Llamado por el TongueController al impactar la lengua.
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
            // Desactivar collider durante el arrastre para evitar colisiones indeseadas
            if (itemCollider != null && type != ItemType.HeavyBlock)
            {
                itemCollider.enabled = false;
            }

            // Desenganchar de un padre si era el escudo de un enemigo
            if (transform.parent != null)
            {
                transform.SetParent(null);
            }

            // Trayecto de vuelo hacia la rana / objetivo
            while (pullTarget != null && Vector2.Distance(transform.position, pullTarget.position) > stopDistance)
            {
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    pullTarget.position,
                    pullSpeed * Time.deltaTime
                );
                yield return null;
            }

            // Lógica al llegar al jugador
            onCollected?.Invoke();

            switch (type)
            {
                case ItemType.Collectible:
                    // TODO: Notificar al inventario / GameManager
                    Destroy(gameObject);
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