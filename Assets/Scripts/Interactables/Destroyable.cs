namespace FrogGame.Gameplay
{
    using UnityEngine;

    /// <summary>
    /// Handles destroyable objects in the scene by checking 2D trigger collisions against a specified layer.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class Destroyable : MonoBehaviour
    {
        [Header("Layer Detection")]
        [SerializeField] private LayerMask arrowLayer;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (((1 << collision.gameObject.layer) & arrowLayer) != 0)
            {
                Destroy(gameObject);
            }
        }
    }
}