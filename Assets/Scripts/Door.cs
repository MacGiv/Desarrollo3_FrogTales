namespace FrogGame.World
{
    using UnityEngine;

    /// <summary>
    /// Interactive door supporting single or multi-switch puzzle requirements.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Door : MonoBehaviour
    {
        [Header("Puzzle Settings")]
        [SerializeField] private int requiredSwitches = 1;
        [SerializeField] private bool isOpen = false;

        [Header("Visual References")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite openSprite;
        [SerializeField] private Sprite closedSprite;

        private Collider2D doorCollider;
        private int currentActiveSwitches = 0;

        private void Awake()
        {
            doorCollider = GetComponent<Collider2D>();
            UpdateDoorState();
        }

        /// <summary>
        /// Call this when any connected switch gets pressed.
        /// </summary>
        public void RegisterSwitchPressed()
        {
            currentActiveSwitches++;
            EvaluateState();
        }

        /// <summary>
        /// Call this when any connected switch gets released.
        /// </summary>
        public void RegisterSwitchReleased()
        {
            currentActiveSwitches = Mathf.Max(0, currentActiveSwitches - 1);
            EvaluateState();
        }

        private void EvaluateState()
        {
            bool shouldBeOpen = currentActiveSwitches >= requiredSwitches;

            if (shouldBeOpen != isOpen)
            {
                isOpen = shouldBeOpen;
                UpdateDoorState();
            }
        }

        private void UpdateDoorState()
        {
            if (doorCollider != null)
            {
                doorCollider.enabled = !isOpen;
            }

            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = isOpen ? openSprite : closedSprite;
            }
        }
    }
}
