namespace FrogGame.UI
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using FrogGame.Gameplay;

    /// <summary>
    /// Controls the visibility and content of the sign UI panel by listening 
    /// to interaction events triggered.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class SignUIController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Image displayImage;

        private CanvasGroup canvasGroup;

        private void Awake() => canvasGroup = GetComponent<CanvasGroup>();

        private void OnEnable()
        {
            SignDetector.OnPlayerEnteredSign += Show;
            SignDetector.OnPlayerExitedSign += Hide;
        }

        private void OnDisable()
        {
            SignDetector.OnPlayerEnteredSign -= Show;
            SignDetector.OnPlayerExitedSign -= Hide;
        }

        private void Start() => Hide();

        private void Show(string title, string text, Sprite image)
        {
            if (titleText != null)
            {
                titleText.text = title;
            }

            if (descriptionText != null)
            {
                descriptionText.text = text;
            }

            if (displayImage != null)
            {
                displayImage.sprite = image;
            }

            SetVisibility(true);
        }

        private void Hide() => SetVisibility(false);

        private void SetVisibility(bool isVisible)
        {
            if (canvasGroup == null) return;

            canvasGroup.alpha = isVisible ? 1f : 0f;
            canvasGroup.blocksRaycasts = isVisible;
            canvasGroup.interactable = isVisible;
        }
    }
}