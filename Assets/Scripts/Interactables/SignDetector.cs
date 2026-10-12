namespace FrogGame.Gameplay
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Detects when a player enters or exits the sign's interaction zone 
    /// and broadcasts the sign's data using static events.
    /// </summary>
    public class SignDetector : MonoBehaviour
    {
        [Header("Sign Configuration")]
        [SerializeField] private string title;
        [TextArea(3, 5)]
        [SerializeField] private string description;
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
                OnPlayerEnteredSign?.Invoke(title, description, image);
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