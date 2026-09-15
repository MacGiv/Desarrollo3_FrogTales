using UnityEngine;
using UnityEngine.InputSystem;

namespace FrogGame.Gameplay
{
    /// <summary>
    /// Reads inputs using Unity's New Input System.
    /// Maneja la lectura de entradas con el nuevo sistema de inputs de Unity.
    /// </summary>
    public class PlayerInputHandler : MonoBehaviour
    {
        [Header("Input Action References")]
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference tongueAction;
        [SerializeField] private InputActionReference arrowAction;

        public Vector2 MoveInput { get; private set; }
        public bool TonguePressed { get; private set; }
        public bool ArrowPressed { get; private set; }

        private void OnEnable()
        {
            moveAction.action.Enable();
            tongueAction.action.Enable();
            arrowAction.action.Enable();

            tongueAction.action.performed += OnTonguePerformed;
            arrowAction.action.performed += OnArrowPerformed;
        }

        private void OnDisable()
        {
            moveAction.action.Disable();
            tongueAction.action.Disable();
            arrowAction.action.Disable();

            tongueAction.action.performed -= OnTonguePerformed;
            arrowAction.action.performed -= OnArrowPerformed;
        }

        private void Update()
        {
            MoveInput = moveAction.action.ReadValue<Vector2>();
        }

        private void OnTonguePerformed(InputAction.CallbackContext context) => TonguePressed = true;
        private void OnArrowPerformed(InputAction.CallbackContext context) => ArrowPressed = true;

        public void ConsumeTongueInput() => TonguePressed = false;
        public void ConsumeArrowInput() => ArrowPressed = false;
    }
}

