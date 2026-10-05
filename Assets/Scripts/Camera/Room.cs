namespace FrogGame.Camera
{
    using UnityEngine;

    [RequireComponent(typeof(BoxCollider2D))]
    public class Room : MonoBehaviour
    {
        [Header("Room Setup")]
        [SerializeField] private RoomType roomType = RoomType.Fixed;

        private BoxCollider2D roomCollider;

        public RoomType Type => roomType;
        public BoxCollider2D Collider => roomCollider;

        private void Awake()
        {
            roomCollider = GetComponent<BoxCollider2D>();
        }

        /// <summary>
        /// Calculates target camera position based on RoomType and camera screen bounds.
        /// Calcula la posición ideal de la cámara según el tipo de sala y el aspecto de la pantalla.
        /// </summary>
        public Vector3 GetClampedCameraPosition(Vector3 playerPos, UnityEngine.Camera mainCam)
        {
            if (roomCollider == null) roomCollider = GetComponent<BoxCollider2D>();

            Bounds bounds = roomCollider.bounds;
            Vector3 targetPos = bounds.center;

            float camHeight = mainCam.orthographicSize * 2f;
            float camWidth = camHeight * mainCam.aspect;

            switch (roomType)
            {
                case RoomType.Fixed:
                    // Se queda fija en el centro de la sala
                    targetPos = bounds.center;
                    break;

                case RoomType.Horizontal:
                    targetPos.y = bounds.center.y;

                    float minX = bounds.min.x + (camWidth / 2f);
                    float maxX = bounds.max.x - (camWidth / 2f);

                    // Si la sala es más angosta que la pantalla, se centra automáticamente
                    targetPos.x = (minX > maxX) ? bounds.center.x : Mathf.Clamp(playerPos.x, minX, maxX);
                    break;

                case RoomType.Vertical:
                    targetPos.x = bounds.center.x;

                    float minY = bounds.min.y + (camHeight / 2f);
                    float maxY = bounds.max.y - (camHeight / 2f);

                    // Si la sala es más baja que la pantalla, se centra automáticamente
                    targetPos.y = (minY > maxY) ? bounds.center.y : Mathf.Clamp(playerPos.y, minY, maxY);
                    break;
            }

            targetPos.z = 0f;
            return targetPos;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                RoomManager.Instance?.RequestRoomChange(this, collision.transform);
            }
        }

        private void OnDrawGizmos()
        {
            BoxCollider2D col = GetComponent<BoxCollider2D>();
            if (col == null) return;

            Gizmos.color = roomType switch
            {
                RoomType.Fixed => Color.red,
                RoomType.Horizontal => Color.green,
                RoomType.Vertical => Color.blue,
                _ => Color.white
            };

            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
        }
    }
}