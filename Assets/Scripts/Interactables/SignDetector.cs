namespace FrogGame.Gameplay
{
    using UnityEngine;

    public class SignDetector : MonoBehaviour
    {
        [Header("Layer Detection")]
        [SerializeField] private LayerMask playerLayer;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (((1 << collision.gameObject.layer) & playerLayer) != 0)
            {
                Debug.Log("Player detected!");
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (((1 << collision.gameObject.layer) & playerLayer) != 0)
            {
                Debug.Log("Player exit!");
            }
        }
    }
}