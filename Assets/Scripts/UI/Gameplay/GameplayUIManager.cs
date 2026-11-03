namespace FrogGame.UI
{
    using UnityEngine;
    using UnityEngine.InputSystem;
    using UnityEngine.SceneManagement;
    using FrogGame.Core;

    public class GameplayUIManager : MonoBehaviour
    {
        [Header("Controller References")]
        [SerializeField] private GameController gameController;

        [Header("Panels References")]
        [SerializeField] private PausePanelController pausePanel;
        [SerializeField] private OptionsPanelController optionsPanel;

        [Header("Input Setup")]
        [SerializeField] private InputActionAsset inputActions;

        private InputActionMap playerMap;
        private InputActionMap uiMap;

        private void Awake()
        {
            if (inputActions != null)
            {
                playerMap = inputActions.FindActionMap("Player");
                uiMap = inputActions.FindActionMap("UI");
            }

            SubscribeEvents();
        }

        private void Start() => HideAllPanels();

        private void OnDestroy() => UnsubscribeEvents();

        #region Event Subscriptions

        private void SubscribeEvents()
        {
            if (gameController != null)
            {
                gameController.OnPauseToggled += HandlePauseToggled;
            }

            pausePanel.OnContinuePressed += HandleContinuePressed;
            pausePanel.OnOptionsPressed += ShowOptionsPanel;
            pausePanel.OnQuitToMenuPressed += HandleQuitToMenuPressed;

            optionsPanel.OnBackPressed += ShowPausePanel;

        }

        private void UnsubscribeEvents()
        {
            if (gameController != null)
            {
                gameController.OnPauseToggled -= HandlePauseToggled;
            }

            pausePanel.OnContinuePressed -= HandleContinuePressed;
            pausePanel.OnOptionsPressed -= ShowOptionsPanel;
            pausePanel.OnQuitToMenuPressed -= HandleQuitToMenuPressed;

            optionsPanel.OnBackPressed -= ShowPausePanel;
        }

        #endregion

        #region Panel Navigation

        private void ShowPausePanel()
        {
            pausePanel.Show();
            optionsPanel.Hide();

            SetPauseState(true);
        }

        private void ShowOptionsPanel()
        {
            pausePanel.Hide();
            optionsPanel.Show();
        }

        private void HideAllPanels()
        {
            pausePanel.Hide();
            optionsPanel.Hide();

            SetPauseState(false);
        }

        private void SetPauseState(bool isPaused)
        {
            Time.timeScale = isPaused ? 0f : 1f;

            if (isPaused)
            {
                playerMap?.Disable();
                uiMap?.Enable();
            }
            else
            {
                playerMap?.Enable();
                uiMap?.Disable();
            }
        }

        #endregion

        #region Event Handlers

        private void HandlePauseToggled(bool isPaused)
        {
            if (isPaused)
            {
                ShowPausePanel();
            }
            else
            {
                HideAllPanels();
            }
        }

        private void HandleContinuePressed() => HideAllPanels();

        private void HandleQuitToMenuPressed()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }

        #endregion
    }
}