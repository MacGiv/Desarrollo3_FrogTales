namespace FrogGame.World
{
    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// Pressure plate that triggers events when stepped on by player or pushable blocks.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class FloorSwitch : MonoBehaviour
    {
        [Header("Switch Logic")]
        [SerializeField] private bool stayPressed = false;
        [SerializeField] private LayerMask triggerLayers;

        [Header("Events")]
        [SerializeField] private UnityEvent onPressed;
        [SerializeField] private UnityEvent onReleased;

        [Header("Visual Feedback (Optional)")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite pressedSprite;
        [SerializeField] private Sprite unpressedSprite;

        private int occupantsCount = 0;
        public bool IsPressed => occupantsCount > 0;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            int colLayer = 1 << collision.gameObject.layer;

            if ((colLayer & triggerLayers) != 0)
            {
                occupantsCount++;

                if (occupantsCount == 1)
                {
                    OnSwitchPressed();
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (stayPressed) return;

            int colLayer = 1 << collision.gameObject.layer;

            if ((colLayer & triggerLayers) != 0)
            {
                occupantsCount--;
                occupantsCount = Mathf.Max(0, occupantsCount);

                if (occupantsCount == 0)
                {
                    OnSwitchReleased();
                }
            }
        }

        private void OnSwitchPressed()
        {
            if (spriteRenderer != null && pressedSprite != null)
            {
                spriteRenderer.sprite = pressedSprite;
            }

            onPressed?.Invoke();
        }

        private void OnSwitchReleased()
        {
            if (spriteRenderer != null && unpressedSprite != null)
            {
                spriteRenderer.sprite = unpressedSprite;
            }

            onReleased?.Invoke();
        }
    }
}
