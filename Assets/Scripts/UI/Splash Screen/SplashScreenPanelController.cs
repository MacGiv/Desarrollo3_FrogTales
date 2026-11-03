namespace FrogGame.UI
{
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.InputSystem;

    public class SplashScreenPanelController : MonoBehaviour
    {
        [Header("Input System Setup")]
        [SerializeField] private InputActionReference submitAction;

        private void OnEnable()
        {
            if (submitAction != null)
            {
                submitAction.action.performed += OnSubmitPerformed;
                submitAction.action.Enable();
            }
        }

        private void OnDisable()
        {
            if (submitAction != null)
            {
                submitAction.action.performed -= OnSubmitPerformed;
                submitAction.action.Disable();
            }
        }

        private void OnSubmitPerformed(InputAction.CallbackContext context) => LoadNextScene();

        private void LoadNextScene() => SceneManager.LoadScene("MainMenu");
    }
}