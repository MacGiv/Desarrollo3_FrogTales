namespace FrogGame.Gameplay
{
    using UnityEngine;

    /// <summary>
    /// Handles 8-directional physical movement and 4-cardinal facing calculations for animations.
    /// Maneja el movimiento físico en 8 direcciones y calcula la orientación cardinal en 4 direcciones para animaciones.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovementHandler : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float moveSpeed = 6f;

        private Rigidbody2D rb;
        public Vector2 FacingDirection { get; private set; } = Vector2.down; // Dirección por defecto mirando al frente

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        public void SetVelocity(Vector2 input)
        {
            // Movimiento físico en 8 direcciones
            rb.linearVelocity = input.normalized * moveSpeed;

            if (input != Vector2.zero)
            {
                // Ajusta la dirección de la mirada a 4 direcciones cardinales (N, S, E, O)
                if (Mathf.Abs(input.x) >= Mathf.Abs(input.y))
                {
                    FacingDirection = new Vector2(Mathf.Sign(input.x), 0f);
                }
                else
                {
                    FacingDirection = new Vector2(0f, Mathf.Sign(input.y));
                }
            }
        }

        public void Stop() => rb.linearVelocity = Vector2.zero;
    }
}