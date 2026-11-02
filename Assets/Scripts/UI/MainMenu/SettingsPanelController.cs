namespace FrogGame.UI
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;
    using FrogGame.Core;

    public class SettingsPanelController : UIPanel
    {
        [Header("Button References")]
        [SerializeField] private Button backButton;

        public event Action OnBackPressed;

        protected override void Awake()
        {
            base.Awake();

            backButton.onClick.AddListener(HandleBackClicked);
        }

        private void OnDestroy() => backButton.onClick.RemoveListener(HandleBackClicked);

        private void HandleBackClicked() => OnBackPressed?.Invoke();
    }
}