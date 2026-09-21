namespace FrogGame.Enemies
{
    using UnityEngine;
    using FrogGame.Core;

    /// <summary>
    /// Brain & FSM Controller for the Fox enemy.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class FoxBrain : MonoBehaviour, IDamageable, IStunnable
    {
        [Header("Stats")]
        [SerializeField] private int maxHealth = 3;
        [SerializeField] private float patrolSpeed = 2.5f;
        [SerializeField] private float chaseSpeed = 4.5f;

        [Header("Detection & Combat Settings")]
        [SerializeField] private float detectionRadius = 5f;
        [SerializeField] private float attackRange = 1f;
        [SerializeField] private float attackCooldown = 1.5f;
        [SerializeField] private LayerMask playerLayer;

        [Header("Patrol Settings")]
        [SerializeField] private Transform[] waypoints;

        private Rigidbody2D rb;
        private int currentHealth;

        public FiniteStateMachine FSM { get; private set; }
        public Rigidbody2D Rb => rb;
        public float PatrolSpeed => patrolSpeed;
        public float ChaseSpeed => chaseSpeed;
        public float DetectionRadius => detectionRadius;
        public float AttackRange => attackRange;
        public float AttackCooldown => attackCooldown;
        public Transform[] Waypoints => waypoints;
        public Transform TargetPlayer { get; private set; }
        // States
        public FoxPatrolState PatrolState { get; private set; }
        public FoxChaseState ChaseState { get; private set; }
        public FoxAttackState AttackState { get; private set; }
        public FoxStunnedState StunnedState { get; private set; }

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            currentHealth = maxHealth;

            FSM = new FiniteStateMachine();

            PatrolState = new FoxPatrolState(this);
            ChaseState = new FoxChaseState(this);
            AttackState = new FoxAttackState(this);
            StunnedState = new FoxStunnedState(this);
        }

        private void Start()
        {
            FSM.Initialize(PatrolState);
        }

        private void Update()
        {
            DetectPlayer();
            FSM.CurrentState.LogicUpdate();
        }

        private void FixedUpdate()
        {
            FSM.CurrentState.PhysicsUpdate();
        }

        /// <summary>
        /// Detects player within range using OverlapCircle.
        /// </summary>
        private void DetectPlayer()
        {
            Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
            if (hit != null)
            {
                TargetPlayer = hit.transform;
            }
            else
            {
                TargetPlayer = null;
            }
        }

        public void TakeDamage(int damageAmount)
        {
            currentHealth -= damageAmount;
            Debug.Log($"{gameObject.name} took {damageAmount} damage. Health left: {currentHealth}");

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public void ApplyStun(float duration)
        {
            StunnedState.SetStunDuration(duration);
            FSM.ChangeState(StunnedState);
        }

        private void Die()
        {
            // TODO: Efectos visuales, drops o eventos de muerte
            Destroy(gameObject);
        }

        private void OnDrawGizmosSelected()
        {
            // Visualización de rangos en el editor
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}