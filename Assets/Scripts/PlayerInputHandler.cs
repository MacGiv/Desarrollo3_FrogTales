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
        [SerializeField] private InputActionReference stunAction;

        public Vector2 MoveInput { get; private set; }
        public bool TonguePressed { get; private set; }
        public bool ArrowPressed { get; private set; }
        public bool StunPressed { get; private set; }

        private void OnEnable()
        {
            if (moveAction != null) moveAction.action.Enable();

            if (tongueAction != null)
            {
                tongueAction.action.Enable();
                tongueAction.action.performed += OnTonguePerformed;
            }

            if (arrowAction != null)
            {
                arrowAction.action.Enable();
                arrowAction.action.performed += OnArrowPerformed;
            }

            if (stunAction != null)
            {
                stunAction.action.Enable();
                stunAction.action.performed += OnStunPerformed;
            }
        }

        private void OnDisable()
        {
            if (moveAction != null) moveAction.action.Disable();

            if (tongueAction != null)
            {
                tongueAction.action.Disable();
                tongueAction.action.performed -= OnTonguePerformed;
            }

            if (arrowAction != null)
            {
                arrowAction.action.Disable();
                arrowAction.action.performed -= OnArrowPerformed;
            }

            if (stunAction != null)
            {
                stunAction.action.Disable();
                stunAction.action.performed -= OnStunPerformed;
            }
        }

        private void Update()
        {
            if (moveAction != null)
            {
                MoveInput = moveAction.action.ReadValue<Vector2>();
            }
        }

        private void OnTonguePerformed(InputAction.CallbackContext context) => TonguePressed = true;
        private void OnArrowPerformed(InputAction.CallbackContext context) => ArrowPressed = true;
        private void OnStunPerformed(InputAction.CallbackContext context) => StunPressed = true;

        public void ConsumeTongueInput() => TonguePressed = false;
        public void ConsumeArrowInput() => ArrowPressed = false;
        public void ConsumeStunInput() => StunPressed = false;
    }
}