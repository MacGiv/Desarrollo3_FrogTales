namespace FrogGame.UI
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using FrogGame.Gameplay;

    /// <summary>
    /// Controls the visibility, text, and frame animation of the sign UI panel 
    /// by listening to interaction events
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class SignUIController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Image displayImage;

        private CanvasGroup canvasGroup;
        private Sprite[] currentSequence;
        private float frameRate = 6f;
        private int currentFrame = 0;
        private float frameTimer = 0f;
        private bool isAnimating = false;

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

        private void Update()
        {
            if (!isAnimating || currentSequence == null || currentSequence.Length <= 1) return;

            frameTimer += Time.deltaTime;

            if (frameTimer >= (1f / frameRate))
            {
                frameTimer -= (1f / frameRate);
                currentFrame = (currentFrame + 1) % currentSequence.Length;

                if (displayImage != null)
                {
                    displayImage.sprite = currentSequence[currentFrame];
                }
            }
        }

        private void Show(string title, string text, Sprite[] sequence, float fps)
        {
            if (titleText != null)
            {
                titleText.text = title;
            }

            if (descriptionText != null)
            {
                descriptionText.text = text;
            }

            currentSequence = sequence;
            frameRate = fps > 0 ? fps : 6f;
            currentFrame = 0;
            frameTimer = 0f;

            if (displayImage != null)
            {
                bool hasSprites = currentSequence != null && currentSequence.Length > 0;
                displayImage.enabled = hasSprites;

                if (hasSprites)
                {
                    displayImage.sprite = currentSequence[0];
                }
            }

            isAnimating = currentSequence != null && currentSequence.Length > 1;

            SetVisibility(true);
        }

        private void Hide()
        {
            isAnimating = false;
            SetVisibility(false);
        }

        private void SetVisibility(bool isVisible)
        {
            if (canvasGroup == null) return;

            canvasGroup.alpha = isVisible ? 1f : 0f;
            canvasGroup.blocksRaycasts = isVisible;
            canvasGroup.interactable = isVisible;
        }
    }
}