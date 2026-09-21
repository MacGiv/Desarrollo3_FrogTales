namespace FrogGame.Gameplay
{
    using UnityEngine;
    using FrogGame.Core;

    /// <summary>
    /// Special projectile (booger/snot) that stuns enemies on impact.
    /// Proyectil especial (moco) que aturde a los enemigos al impactar.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class StunProjectile : MonoBehaviour
    {
        [Header("Projectile Settings")]
        [SerializeField] private float speed = 10f;
        [SerializeField] private float stunDuration = 3f;
        [SerializeField] private float maxLifetime = 4f;

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
        /// Initializes projectile direction and velocity.
        /// Inicializa la dirección y velocidad del proyectil.
        /// </summary>
        public void Setup(Vector2 direction)
        {
            Vector2 normalizedDir = direction.normalized;
            rb.linearVelocity = normalizedDir * speed;

            float angle = Mathf.Atan2(normalizedDir.y, normalizedDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            int colLayer = 1 << collision.gameObject.layer;

            // 1. Check obstacle collision / Choque contra pared u obstáculo
            if ((colLayer & obstacleLayer) != 0)
            {
                Destroy(gameObject);
                return;
            }

            // 2. Check enemy/stunnable collision / Impacto contra enemigo o entidad aturdible
            if ((colLayer & enemyLayer) != 0)
            {
                if (collision.TryGetComponent<IStunnable>(out var stunnable))
                {
                    stunnable.ApplyStun(stunDuration);
                }

                Destroy(gameObject);
            }
        }
    }
}