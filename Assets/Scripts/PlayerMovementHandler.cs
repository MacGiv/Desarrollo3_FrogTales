using UnityEngine;

namespace FrogGame.Gameplay
{

    /// <summary>
    /// Handles 8-directional physical movement while snapping facing rotation to 4 cardinal directions.
    /// Maneja el movimiento físico en 8 direcciones mientras ajusta la rotación a 4 direcciones cardinales.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovementHandler : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float moveSpeed = 6f;

        private Rigidbody2D rb;
        public Vector2 FacingDirection { get; private set; } = Vector2.right;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        public void SetVelocity(Vector2 input)
        {
            // 8-directional physical movement / Movimiento físico en 8 direcciones
            rb.linearVelocity = input.normalized * moveSpeed;

            if (input != Vector2.zero)
            {
                // Snap facing direction to 4 cardinal directions (N, S, E, W)
                // Ajusta la dirección de la mirada a 4 direcciones cardinales (N, S, E, O)
                if (Mathf.Abs(input.x) >= Mathf.Abs(input.y))
                {
                    FacingDirection = new Vector2(Mathf.Sign(input.x), 0f);
                }
                else
                {
                    FacingDirection = new Vector2(0f, Mathf.Sign(input.y));
                }

                float angle = Mathf.Atan2(FacingDirection.y, FacingDirection.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, angle);
            }
        }

        public void Stop() => rb.linearVelocity = Vector2.zero;
    }
}

