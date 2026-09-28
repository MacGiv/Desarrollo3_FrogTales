namespace FrogGame.World
{
    using UnityEngine;
    using FrogGame.Camera;

    /// <summary>
    /// Triggers a camera room transition when the player enters this zone.
    /// Activa la transición de la cámara al entrar la rana en esta zona.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class RoomZone : MonoBehaviour
    {
        [Header("Room Collider")]
        [SerializeField] private Collider2D roomBounds;

        private void Awake()
        {
            if (roomBounds == null)
            {
                roomBounds = GetComponent<Collider2D>();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                RoomManager.Instance?.ChangeRoom(roomBounds);
            }
        }
    }
}
