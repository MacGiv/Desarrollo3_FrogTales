namespace FrogGame.Enemies
{
    using UnityEngine;
    using FrogGame.Core;

    /// <summary>
    /// Brain & FSM Controller for the flying Small Owl enemy.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class SmallOwlBrain : MonoBehaviour, IDamageable, IStunnable
    {
        [Header("Stats")]
        [SerializeField] private int maxHealth = 2;
        [SerializeField] private float flySpeed = 3f;
        [SerializeField] private float swoopSpeed = 9f;

        [Header("Combat & Detection")]
        [SerializeField] private float detectionRadius = 6f;
        [SerializeField] private float attackCooldown = 2f;
        [SerializeField] private int attackDamage = 1;
        [SerializeField] private LayerMask playerLayer;

        [Header("Patrol Settings")]
        [SerializeField] private Transform[] waypoints;

        private Rigidbody2D rb;
        private int currentHealth;

        public FiniteStateMachine FSM { get; private set; }
        public Rigidbody2D Rb => rb;
        public float FlySpeed => flySpeed;
        public float SwoopSpeed => swoopSpeed;
        public float DetectionRadius => detectionRadius;
        public float AttackCooldown => attackCooldown;
        public int AttackDamage => attackDamage;
        public Transform[] Waypoints => waypoints;
        public Transform TargetPlayer { get; private set; }

        // States
        public SmallOwlPatrolState PatrolState { get; private set; }
        public SmallOwlSwoopState SwoopState { get; private set; }
        public SmallOwlStunnedState StunnedState { get; private set; }

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            currentHealth = maxHealth;

            FSM = new FiniteStateMachine();

            PatrolState = new SmallOwlPatrolState(this);
            SwoopState = new SmallOwlSwoopState(this);
            StunnedState = new SmallOwlStunnedState(this);
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

        private void DetectPlayer()
        {
            Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
            TargetPlayer = hit != null ? hit.transform : null;
        }

        public void TakeDamage(int damageAmount)
        {
            currentHealth -= damageAmount;
            if (currentHealth <= 0)
            {
                Destroy(gameObject);
            }
        }

        public void ApplyStun(float duration)
        {
            StunnedState.SetStunDuration(duration);
            FSM.ChangeState(StunnedState);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Apply contact damage during swoop
            if (FSM.CurrentState == SwoopState)
            {
                if (collision.TryGetComponent<IDamageable>(out var damageable) && collision.CompareTag("Player"))
                {
                    damageable.TakeDamage(attackDamage);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}