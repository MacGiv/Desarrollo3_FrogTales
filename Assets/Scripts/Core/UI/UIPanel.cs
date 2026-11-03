namespace FrogGame.Core
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;

    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIPanel : MonoBehaviour
    {
        [Header("Focus Button Setup")]
        [SerializeField] protected Button defaultSelected;

        protected CanvasGroup CanvasGroup { get; private set; }

        protected virtual void Awake() => CanvasGroup = GetComponent<CanvasGroup>();

        public virtual void Show()
        {
            CanvasGroup.alpha = 1.0f;
            CanvasGroup.interactable = true;
            CanvasGroup.blocksRaycasts = true;

            Focus();
        }

        public virtual void Hide()
        {
            CanvasGroup.alpha = 0.0f;
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
        }

        public virtual void Focus()
        {
            if (defaultSelected == null) return;

            StopAllCoroutines();
            StartCoroutine(SetFocusRoutine());
        }

        private IEnumerator SetFocusRoutine()
        {
            EventSystem.current?.SetSelectedGameObject(null);

            yield return new WaitForEndOfFrame();

            if (defaultSelected != null && defaultSelected.gameObject.activeInHierarchy)
            {
                EventSystem.current?.SetSelectedGameObject(defaultSelected.gameObject);
            }
        }
    }
}