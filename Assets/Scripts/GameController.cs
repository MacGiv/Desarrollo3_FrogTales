namespace FrogGame.Core
{
    using System;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using UnityEngine.SceneManagement;
    using FrogGame.Gameplay;

    /// <summary>
    /// Debug & scene controller for restarting, quitting, and pausing using Unity's New Input System.
    /// </summary>
    public class GameController : MonoBehaviour
    {
        [Header("UI Canvas")]
        [SerializeField] private GameObject pauseCanvas;

        [Header("Restart Settings")]
        [SerializeField] private float deathRestartDelay = 1f;

        [Header("System Input Action References")]
        [SerializeField] private InputActionReference pauseAction;
        [SerializeField] private InputActionReference restartAction;

        public bool IsPaused { get; private set; }

        public event Action<bool> OnPauseToggled;

        private void OnEnable()
        {
            PlayerHealthSystem.OnPlayerDied += HandlePlayerDied;

            if (pauseAction != null)
            {
                pauseAction.action.Enable();
                pauseAction.action.performed += OnPausePerformed;
            }

            if (restartAction != null)
            {
                restartAction.action.Enable();
                restartAction.action.performed += OnRestartPerformed;
            }
        }

        private void OnDisable()
        {
            PlayerHealthSystem.OnPlayerDied -= HandlePlayerDied;

            if (pauseAction != null)
            {
                pauseAction.action.performed -= OnPausePerformed;
                pauseAction.action.Disable();
            }

            if (restartAction != null)
            {
                restartAction.action.performed -= OnRestartPerformed;
                restartAction.action.Disable();
            }
        }

        private void OnPausePerformed(InputAction.CallbackContext context)
        {
            IsPaused = !IsPaused;
            OnPauseToggled?.Invoke(IsPaused);
        }

        private void OnRestartPerformed(InputAction.CallbackContext context)
        {
            RestartScene();
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