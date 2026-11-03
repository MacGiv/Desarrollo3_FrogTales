namespace FrogGame.UI
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;
    using FrogGame.Core;

    public class MainPanelController : UIPanel
    {
        [Header("Button References")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button exitButton;

        public event Action OnPlayPressed;
        public event Action OnOptionsPressed;
        public event Action OnCreditsPressed;
        public event Action OnExitPressed;

        protected override void Awake()
        {
            base.Awake();

            playButton.onClick.AddListener(HandlePlayClicked);
            optionsButton.onClick.AddListener(HandleOptionsClicked);
            creditsButton.onClick.AddListener(HandleCreditsClicked);
            exitButton.onClick.AddListener(HandleExitClicked);
        }

        private void Start()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void OnDestroy()
        {
            playButton.onClick.RemoveListener(HandlePlayClicked);
            optionsButton.onClick.RemoveListener(HandleOptionsClicked);
            creditsButton.onClick.RemoveListener(HandleCreditsClicked);
            exitButton.onClick.RemoveListener(HandleExitClicked);
        }

        private void HandlePlayClicked() => OnPlayPressed?.Invoke();

        private void HandleOptionsClicked() => OnOptionsPressed?.Invoke();

        private void HandleCreditsClicked() => OnCreditsPressed?.Invoke();

        private void HandleExitClicked() => OnExitPressed?.Invoke();
    }
}