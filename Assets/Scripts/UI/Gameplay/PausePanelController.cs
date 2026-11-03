namespace FrogGame.UI
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;
    using FrogGame.Core;

    public class PausePanelController : UIPanel
    {
        [Header("Button References")]
        [SerializeField] private Button continueButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button quitToMenuButton;

        public event Action OnContinuePressed;
        public event Action OnOptionsPressed;
        public event Action OnQuitToMenuPressed;

        protected override void Awake()
        {
            base.Awake();

            continueButton.onClick.AddListener(HandleContinueClicked);
            optionsButton.onClick.AddListener(HandleOptionsClicked);
            quitToMenuButton.onClick.AddListener(HandleQuitToMenuClicked);
        }

        private void OnDestroy()
        {
            continueButton.onClick.RemoveListener(HandleContinueClicked);
            optionsButton.onClick.RemoveListener(HandleOptionsClicked);
            quitToMenuButton.onClick.RemoveListener(HandleQuitToMenuClicked);
        }

        private void HandleContinueClicked() => OnContinuePressed?.Invoke();

        private void HandleOptionsClicked() => OnOptionsPressed?.Invoke();

        private void HandleQuitToMenuClicked() => OnQuitToMenuPressed?.Invoke();
    }
}