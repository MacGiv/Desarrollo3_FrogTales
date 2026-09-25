namespace FrogGame.Camera
{
    using UnityEngine;
    using Unity.Cinemachine;

    /// <summary>
    /// Singleton manager that controls camera transitions between room bounds.
    /// </summary>
    public class RoomManager : MonoBehaviour
    {
        public static RoomManager Instance { get; private set; }

        [Header("Cinemachine Integration")]
        [SerializeField] private CinemachineConfiner2D confiner;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        /// <summary>
        /// Updates the Cinemachine confiner bounds to pan smoothly into a new room.
        /// </summary>
        public void ChangeRoom(Collider2D newRoomBounds)
        {
            if (confiner == null || newRoomBounds == null) return;

            if (confiner.BoundingShape2D != newRoomBounds)
            {
                confiner.BoundingShape2D = newRoomBounds;
                // Recalculates the collider's borders so the transition is smooth
                confiner.InvalidateBoundingShapeCache();
            }
        }
    }
}
