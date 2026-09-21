namespace FrogGame.Camera
{
    using UnityEngine;
    using Unity.Cinemachine;

    /// <summary>
    /// Binds the Player transform to the Cinemachine Virtual Camera automatically on Start.
    /// Vincula automáticamente la posición del jugador a la cámara de Cinemachine al iniciar.
    /// </summary>
    [RequireComponent(typeof(CinemachineCamera))] 
    public class CameraFollowBinder : MonoBehaviour
    {
        [Header("Target Search Settings")]
        [SerializeField] private string playerTag = "Player";

        private CinemachineCamera virtualCamera;

        private void Awake()
        {
            virtualCamera = GetComponent<CinemachineCamera>();
        }

        private void Start()
        {
            BindPlayerTarget();
        }

        public void BindPlayerTarget()
        {
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);

            if (player != null)
            {
                virtualCamera.Follow = player.transform;
            }
            else
            {
                Debug.LogWarning("[CameraFollowBinder] Player object with tag 'Player' not found in scene!");
            }
        }
    }
}