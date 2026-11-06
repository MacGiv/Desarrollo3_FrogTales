namespace FrogGame.Gameplay
{
    using UnityEngine;
    using UnityEngine.Tilemaps;

    /// <summary>
    /// Represents an item configured to spawn with a specific probability percentage (0 to 100).
    /// </summary>
    [System.Serializable]
    public struct DropItem
    {
        [SerializeField] private GameObject prefab;

        [Range(0, 100)]
        [SerializeField] private int dropChance;

        public readonly GameObject Prefab => prefab;
        public readonly int DropChance => dropChance;
    }

    /// <summary>
    /// Handles destroyable objects in the scene by detecting 2D trigger collisions against a specified layer,
    /// clearing their Tilemap cell, playing audio/VFX, and conditionally spawning a single drop item based on probability.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class Destroyable : MonoBehaviour
    {
        [Header("Layer Detection")]
        [SerializeField] private LayerMask arrowLayer;

        [Header("Effects")]
        [SerializeField] private GameObject explosionVfxPrefab;

        [Header("Dropper Settings")]
        [SerializeField] private bool isDroper = false;
        [SerializeField] private DropItem[] droppableItems;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (((1 << collision.gameObject.layer) & arrowLayer) != 0)
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayEnvRockDrop(gameObject);
                }

                if (explosionVfxPrefab != null)
                {
                    Instantiate(explosionVfxPrefab, transform.position, Quaternion.identity);
                }

                Tilemap tilemap = GetComponentInParent<Tilemap>();

                if (tilemap != null)
                {
                    Vector3Int cellPosition = tilemap.WorldToCell(transform.position);
                    tilemap.SetTile(cellPosition, null);
                }

                if (isDroper)
                {
                    TrySpawnDrop();
                }

                Destroy(gameObject);
            }
        }

        private void TrySpawnDrop()
        {
            if (droppableItems == null || droppableItems.Length == 0) return;

            foreach (DropItem dropItem in droppableItems)
            {
                if (dropItem.Prefab == null) continue;

                int randomValue = Random.Range(0, 100);

                if (randomValue < dropItem.DropChance)
                {
                    Instantiate(dropItem.Prefab, transform.position, Quaternion.identity);

                    break;
                }
            }
        }
    }
}