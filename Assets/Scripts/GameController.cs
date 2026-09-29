namespace FrogGame.Core
{
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.InputSystem; // <-- Nuevo Input System
    using FrogGame.Gameplay;

    /// <summary>
    /// Temporary debug & scene controller for restarting, quitting, and quick pausing.
    /// Controlador temporal para reinicio, salida rápida y menú de pausa.
    /// </summary>
    public class GameController : MonoBehaviour
    {
        [Header("UI Canvas")]
        [SerializeField] private GameObject pauseCanvas;

        [Header("Restart Settings")]
        [SerializeField] private float deathRestartDelay = 1f;

        private bool isPaused = false;

        private void OnEnable()
        {
            PlayerHealthSystem.OnPlayerDied += HandlePlayerDied;
        }

        private void OnDisable()
        {
            PlayerHealthSystem.OnPlayerDied -= HandlePlayerDied;
        }

        private void Start()
        {
            Time.timeScale = 1f;

            if (pauseCanvas != null)
            {
                pauseCanvas.SetActive(false);
            }
        }

        private void Update()
        {
            // Verificación de seguridad si no hay teclado detectado
            if (Keyboard.current == null) return;

            // R -> Reiniciar Escena
            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                RestartScene();
            }

            // Q -> Cerrar Juego
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                QuitGame();
            }

            // P -> Pausar / Despausar
            if (Keyboard.current.pKey.wasPressedThisFrame)
            {
                TogglePause();
            }
        }

        public void TogglePause()
        {
            isPaused = !isPaused;
            Time.timeScale = isPaused ? 0f : 1f;

            if (pauseCanvas != null)
            {
                pauseCanvas.SetActive(isPaused);
            }
        }

        private void HandlePlayerDied()
        {
            Invoke(nameof(RestartScene), deathRestartDelay);
        }

        public void RestartScene()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void QuitGame()
        {
            Debug.Log("[GameController] Quitting Application...");
            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}