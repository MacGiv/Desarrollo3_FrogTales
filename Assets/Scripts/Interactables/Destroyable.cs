namespace FrogGame.Gameplay
{
    using UnityEngine;
    using UnityEngine.Tilemaps;

    /// <summary>
    /// Handles destroyable objects in the scene by checking 2D trigger collisions against a specified layer
    /// and clearing their Tilemap cell before destruction.
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
                Tilemap tilemap = GetComponentInParent<Tilemap>();

                if (tilemap != null)
                {
                    Vector3Int cellPosition = tilemap.WorldToCell(transform.position);
                    tilemap.SetTile(cellPosition, null);
                }

                Destroy(gameObject);
            }
        }
    }
}