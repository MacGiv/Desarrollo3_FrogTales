namespace FrogGame.UI
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Manages UI screen fade-in and fade-out transitions for scene and interior changes.
    /// Gestiona las transiciones de fade-in y fade-out de pantalla para cambios de escena e interiores.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class ScreenFader : MonoBehaviour
    {
        public static ScreenFader Instance { get; private set; }

        [SerializeField] private CanvasGroup fadeCanvasGroup;
        [SerializeField] private float defaultFadeDuration = 0.5f;

        private Coroutine activeFadeCoroutine;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (fadeCanvasGroup == null)
            {
                fadeCanvasGroup = GetComponent<CanvasGroup>();
            }

            // Ensure full opacity at start if needed, or clear
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.blocksRaycasts = false;
        }

        /// <summary>
        /// Fades the screen to black (opaque).
        /// Funde la pantalla a negro (opaco).
        /// </summary>
        public IEnumerator FadeToBlackRoutine(float duration = -1f)
        {
            float targetDuration = duration > 0 ? duration : defaultFadeDuration;
            yield return StartFadeRoutine(1f, targetDuration);
        }

        /// <summary>
        /// Fades the screen from black to clear (transparent).
        /// Funde la pantalla de negro a transparente.
        /// </summary>
        public IEnumerator FadeToClearRoutine(float duration = -1f)
        {
            float targetDuration = duration > 0 ? duration : defaultFadeDuration;
            yield return StartFadeRoutine(0f, targetDuration);
        }

        private IEnumerator StartFadeRoutine(float targetAlpha, float duration)
        {
            fadeCanvasGroup.blocksRaycasts = targetAlpha > 0.5f;
            float startAlpha = fadeCanvasGroup.alpha;
            float timer = 0f;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / duration);
                yield return null;
            }

            fadeCanvasGroup.alpha = targetAlpha;
            fadeCanvasGroup.blocksRaycasts = targetAlpha > 0.5f;
        }
    }
}