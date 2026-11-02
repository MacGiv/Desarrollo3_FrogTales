namespace FrogGame.UI
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class MainMenuManager : MonoBehaviour
    {
        [Header("Panels References")]
        [SerializeField] private MainPanelController mainPanel;
        [SerializeField] private SettingsPanelController settingsPanel;

        private void Awake() => SubscribeEvents();

        private void Start() => ShowMainPanel();

        private void OnDestroy() => UnsubscribeEvents();

        #region Event Subscriptions

        private void SubscribeEvents()
        {
            mainPanel.OnPlayPressed += HandlePlayPressed;
            mainPanel.OnSettingsPressed += ShowSettingsPanel;
            mainPanel.OnCreditsPressed += ShowCreditsPanel;
            mainPanel.OnExitPressed += HandleExitPressed;

            settingsPanel.OnBackPressed += ShowMainPanel;
        }

        private void UnsubscribeEvents()
        {
            mainPanel.OnPlayPressed -= HandlePlayPressed;
            mainPanel.OnSettingsPressed -= ShowSettingsPanel;
            mainPanel.OnCreditsPressed -= ShowCreditsPanel;
            mainPanel.OnExitPressed -= HandleExitPressed;

            settingsPanel.OnBackPressed -= ShowMainPanel;
        }

        #endregion

        #region Panel Navigation

        private void ShowMainPanel()
        {
            mainPanel.Show();
            settingsPanel.Hide();
        }

        private void ShowSettingsPanel()
        {
            mainPanel.Hide();
            settingsPanel.Show();
        }

        private void ShowCreditsPanel()
        {
            Debug.Log("Show Credits Panel!");
        }

        #endregion

        #region Event Handlers

        private void HandlePlayPressed()
        {
            SceneManager.LoadScene("GameplayTest_Artist_Meseta");
        }

        private void HandleExitPressed()
        {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }

        #endregion
    }
}