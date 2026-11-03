namespace FrogGame.UI
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class MainMenuManager : MonoBehaviour
    {
        [Header("Panels References")]
        [SerializeField] private MainPanelController mainPanel;
        [SerializeField] private OptionsPanelController optionsPanel;
        [SerializeField] private CreditsPanelController creditsPanel;

        private void Awake() => SubscribeEvents();

        private void Start() => ShowMainPanel();

        private void OnDestroy() => UnsubscribeEvents();

        #region Event Subscriptions

        private void SubscribeEvents()
        {
            mainPanel.OnPlayPressed += HandlePlayPressed;
            mainPanel.OnOptionsPressed += ShowOptionsPanel;
            mainPanel.OnCreditsPressed += ShowCreditsPanel;
            mainPanel.OnExitPressed += HandleExitPressed;

            optionsPanel.OnBackPressed += ShowMainPanel;

            creditsPanel.OnBackPressed += ShowMainPanel;
        }

        private void UnsubscribeEvents()
        {
            mainPanel.OnPlayPressed -= HandlePlayPressed;
            mainPanel.OnOptionsPressed -= ShowOptionsPanel;
            mainPanel.OnCreditsPressed -= ShowCreditsPanel;
            mainPanel.OnExitPressed -= HandleExitPressed;

            optionsPanel.OnBackPressed -= ShowMainPanel;

            creditsPanel.OnBackPressed -= ShowMainPanel;
        }

        #endregion

        #region Panel Navigation

        private void ShowMainPanel()
        {
            mainPanel.Show();
            optionsPanel.Hide();
            creditsPanel.Hide();
        }

        private void ShowOptionsPanel()
        {
            mainPanel.Hide();
            optionsPanel.Show();
            creditsPanel.Hide();
        }

        private void ShowCreditsPanel()
        {
            mainPanel.Hide();
            optionsPanel.Hide();
            creditsPanel.Show();
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