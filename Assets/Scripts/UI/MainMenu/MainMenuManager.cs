namespace FrogGame.UI
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class MainMenuManager : MonoBehaviour
    {
        [Header("Panels References")]
        [SerializeField] private MainPanelController mainPanel;

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
        }

        private void UnsubscribeEvents()
        {
            mainPanel.OnPlayPressed -= HandlePlayPressed;
            mainPanel.OnSettingsPressed -= ShowSettingsPanel;
            mainPanel.OnCreditsPressed -= ShowCreditsPanel;
            mainPanel.OnExitPressed -= HandleExitPressed;
        }

        #endregion

        #region Panel Navigation

        private void ShowMainPanel()
        {
            mainPanel.Show();
        }

        private void ShowSettingsPanel()
        {
            Debug.Log("Show Settings Panel!");
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