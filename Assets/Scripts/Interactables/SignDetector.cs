namespace FrogGame.Gameplay
{
    using System;
    using UnityEngine;

    public class SignDetector : MonoBehaviour
    {
        [Header("Sign Configuration")]
        [SerializeField] private string title;
        [TextArea(3, 5)]
        [SerializeField] private string text;
        [SerializeField] private Sprite image;

        [Header("Layer Detection")]
        [SerializeField] private LayerMask playerLayer;

        // Static events for global UI subscription
        public static event Action<string, string, Sprite> OnPlayerEnteredSign;
        public static event Action OnPlayerExitedSign;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (((1 << collision.gameObject.layer) & playerLayer) != 0)
            {
                OnPlayerEnteredSign?.Invoke(title, text, image);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (((1 << collision.gameObject.layer) & playerLayer) != 0)
            {
                OnPlayerExitedSign?.Invoke();
            }
        }
    }
}