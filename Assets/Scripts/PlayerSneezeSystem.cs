namespace FrogGame.Gameplay
{
    using System;
    using UnityEngine;
    using UnityEngine.UI; // Required for Canvas UI Image
    using FrogGame.Core;

    /// <summary>
    /// Handles the area-of-effect (AoE) sneeze attack, applying damage, knockback, and stun to enemies.
    /// </summary>
    public class PlayerSneezeHandler : MonoBehaviour
    {
        [Header("Sneeze Settings")]
        [SerializeField] private float range = 2.0f; // ~2 tiles forward
        [SerializeField] private float areaWidth = 1.5f; // AoE width
        [SerializeField] private int damage = 1;
        [SerializeField] private float stunDuration = 2.0f;
        [SerializeField] private float knockbackForce = 10.0f;
        [SerializeField] private float cooldown = 2.0f;

        [Header("Layers & Detection")]
        [SerializeField] private LayerMask enemyLayer;

        [Header("Visual & Feedback References")]
        [SerializeField] private GameObject sneezeParticle;
        [SerializeField] private Image readyIndicator; // Updated to UI Image for HUD Canvas

        private float lastSneezeTime = -999f;
        private PlayerMovementHandler movementHandler;

        public bool CanSneeze => Time.time >= lastSneezeTime + cooldown;
        public float CooldownDuration => cooldown;

        // Event for UI cooldown updates (remainingTime, totalCooldown)
        public static event Action<float, float> OnSneezeTriggered;

        private void Awake()
        {
            movementHandler = GetComponent<PlayerMovementHandler>();
        }

        private void Update()
        {
            UpdateIndicator();
        }

        /// <summary>
        /// Executes the sneeze attack in the direction the player is facing.
        /// </summary>
        public bool ExecuteSneeze()
        {
            if (!CanSneeze)
            {
                Debug.Log("[PlayerSneezeHandler] Sneeze on cooldown!");
                return false;
            }

            lastSneezeTime = Time.time;
            OnSneezeTriggered?.Invoke(cooldown, cooldown);

            Vector2 facingDir = movementHandler != null ? movementHandler.FacingDirection : Vector2.right;
            Vector2 attackCenter = (Vector2)transform.position + (facingDir * (range * 0.5f));
            Vector2 boxSize = new Vector2(range, areaWidth);
            float angle = Mathf.Atan2(facingDir.y, facingDir.x) * Mathf.Rad2Deg;

            Debug.Log($"[PlayerSneezeHandler] Sneeze executed in direction: {facingDir}");

            // Detect enemies within the AoE box
            Collider2D[] hits = Physics2D.OverlapBoxAll(attackCenter, boxSize, angle, enemyLayer);

            foreach (var hit in hits)
            {
                // 1. Apply Damage
                if (hit.TryGetComponent<IDamageable>(out var damageable))
                {
                    damageable.TakeDamage(damage);
                }

                // 2. Apply Stun
                if (hit.TryGetComponent<IStunnable>(out var stunnable))
                {
                    stunnable.ApplyStun(stunDuration);
                }

                // 3. Apply Knockback
                if (hit.TryGetComponent<Rigidbody2D>(out var enemyRb))
                {
                    Vector2 knockbackDir = (hit.transform.position - transform.position).normalized;
                    enemyRb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
                }
            }

            // Trigger visual particle effect if assigned
            if (sneezeParticle != null)
            {
                // Rotate parent container around Z axis matching the attack angle
                Quaternion particleRotation = Quaternion.Euler(0f, 0f, angle);

                GameObject particleObj = Instantiate(sneezeParticle, transform.position, particleRotation);

                // Play particle system on child component
                var ps = particleObj.GetComponentInChildren<ParticleSystem>();
                if (ps != null)
                {
                    ps.Play();
                }

                // Automatically destroy clone after 2 seconds to avoid leaks
                Destroy(particleObj, 2.0f);
            }

            return true;
        }

        private void UpdateIndicator()
        {
            if (readyIndicator == null) return;

            // Change UI Image color based on cooldown state
            readyIndicator.color = CanSneeze ? Color.green : Color.red;
        }

        private void OnDrawGizmosSelected()
        {
            Vector2 facingDir = movementHandler != null ? movementHandler.FacingDirection : Vector2.right;
            if (!Application.isPlaying && movementHandler == null) facingDir = Vector2.right;

            Vector2 attackCenter = (Vector2)transform.position + (facingDir * (range * 0.5f));
            Vector2 boxSize = new Vector2(range, areaWidth);
            float angle = Mathf.Atan2(facingDir.y, facingDir.x) * Mathf.Rad2Deg;

            Gizmos.color = new Color(0f, 1f, 0.8f, 0.35f);
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(attackCenter, Quaternion.Euler(0f, 0f, angle), Vector3.one);
            Gizmos.DrawCube(Vector3.zero, boxSize);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(Vector3.zero, boxSize);
            Gizmos.matrix = oldMatrix;
        }
    }
}