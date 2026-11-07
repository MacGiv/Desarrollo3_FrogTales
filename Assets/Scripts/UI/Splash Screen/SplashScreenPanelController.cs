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
                submitAction.action.Enable();
            }
        }

        private void OnDisable()
        {
            if (submitAction != null)
            {
                submitAction.action.Disable();
            }
        }

        private void Update()
        {
            if (submitAction != null && submitAction.action.WasPressedThisFrame())
            {
                LoadNextScene();
            }
        }

        private void LoadNextScene() => SceneManager.LoadScene("MainMenu");
    }
}