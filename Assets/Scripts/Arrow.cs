namespace FrogGame.Gameplay
{
    using UnityEngine;
    using FrogGame.Core;

    /// <summary>
    /// Projectile script that moves in a direction and damages enemies.
    /// Script del proyectil que avanza, inflige daño a enemigos.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class Arrow : MonoBehaviour
    {
        [Header("Projectile Settings")]
        [SerializeField] private float speed = 12f;
        [SerializeField] private int damage = 1;
        [SerializeField] private float maxLifetime = 3f;

        [Header("Collision Layers")]
        [SerializeField] private LayerMask obstacleLayer;
        [SerializeField] private LayerMask enemyLayer;

        private Rigidbody2D rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            Destroy(gameObject, maxLifetime);
        }

        /// <summary>
        /// Initializes arrow velocity and rotation direction.
        /// Inicializa la velocidad y la orientación de la flecha.
        /// </summary>
        public void Setup(Vector2 direction)
        {
            Vector2 normalizedDir = direction.normalized;

            // Set physics velocity / Asignar velocidad física
            rb.linearVelocity = normalizedDir * speed;

            // Rotate visual arrow to facing direction / Orientar sprite hacia la dirección de vuelo
            float angle = Mathf.Atan2(normalizedDir.y, normalizedDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            int colLayer = 1 << collision.gameObject.layer;

            // 1. Check if hit obstacle/wall / Choque contra pared u obstáculo
            if ((colLayer & obstacleLayer) != 0)
            {
                return;
            }

            // 2. Check if hit enemy / Impacto contra enemigo
            if ((colLayer & enemyLayer) != 0)
            {
                IDamageable damageable = collision.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(damage);
                }

                Destroy(gameObject);
            }
        }
    }
}