namespace FrogGame.Gameplay
{
    using UnityEngine;
    using FrogGame.Core;

    /// <summary>
    /// Central context for Player FSM, components, and animation references.
    /// Contexto central para la FSM, componentes y referencias de animación del jugador.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(PlayerInputHandler))]
    [RequireComponent(typeof(PlayerMovementHandler))]
    [RequireComponent(typeof(PlayerHealthSystem))]
    [RequireComponent(typeof(PlayerAmmoSystem))]
    [RequireComponent(typeof(PlayerWaterDetector))]
    [RequireComponent(typeof(PlayerSneezeHandler))]
    [RequireComponent(typeof(PlayerAnimationHandler))]
    [RequireComponent(typeof(PlayerGrappleCollisionDetector))]
    public class PlayerBrain : MonoBehaviour
    {
        [SerializeField] private GameObject arrowPrefab;
        [SerializeField] private GameObject stunPrefab;
        [SerializeField] private Transform arrowSpawnPoint;
        [SerializeField] private PlayerCarryHandler carryHandler;

        public FiniteStateMachine FSM { get; private set; }
        public Animator Animator { get; private set; }
        public PlayerInputHandler InputHandler { get; private set; }
        public PlayerMovementHandler MovementHandler { get; private set; }
        public PlayerHealthSystem HealthSystem { get; private set; }
        public PlayerAmmoSystem AmmoSystem { get; private set; }
        public TongueController TongueController { get; private set; }
        public PlayerCarryHandler CarryHandler { get; private set; }
        public PlayerWaterDetector WaterDetector { get; private set; }
        public PlayerSneezeHandler SneezeHandler { get; private set; }
        public PlayerAnimationHandler AnimationHandler { get; private set; }
        public PlayerGrappleCollisionDetector GrappleCollisionDetector { get; private set; }

        // States instances / Instancias de Estados
        public PlayerIdleState IdleState { get; private set; }
        public PlayerMoveState MoveState { get; private set; }
        public PlayerShootTongueState TongueState { get; private set; }
        public PlayerArrowAttackState ArrowState { get; private set; }
        public PlayerGrappleState GrappleState { get; private set; }
        public PlayerStunAttackState StunAttackState { get; private set; }
        public PlayerDrownState DrownState { get; private set; }

        public Transform ArrowSpawnPoint => arrowSpawnPoint;
        public GameObject StunPrefab => stunPrefab;
        public GameObject ArrowPrefab => arrowPrefab;

        private void Awake()
        {
            Animator = GetComponent<Animator>();
            InputHandler = GetComponent<PlayerInputHandler>();
            MovementHandler = GetComponent<PlayerMovementHandler>();
            HealthSystem = GetComponent<PlayerHealthSystem>();
            AmmoSystem = GetComponent<PlayerAmmoSystem>();
            TongueController = GetComponent<TongueController>();
            CarryHandler = GetComponent<PlayerCarryHandler>();
            WaterDetector = GetComponent<PlayerWaterDetector>();
            SneezeHandler = GetComponent<PlayerSneezeHandler>();
            AnimationHandler = GetComponent<PlayerAnimationHandler>();
            GrappleCollisionDetector = GetComponent<PlayerGrappleCollisionDetector>();

            FSM = new FiniteStateMachine();
            IdleState = new PlayerIdleState(this);
            MoveState = new PlayerMoveState(this);
            TongueState = new PlayerShootTongueState(this);
            ArrowState = new PlayerArrowAttackState(this);
            GrappleState = new PlayerGrappleState(this);
            StunAttackState = new PlayerStunAttackState(this);
            DrownState = new PlayerDrownState(this);
        }

        private void Start()
        {
            FSM.Initialize(IdleState);
        }

        private void Update()
        {
            FSM.CurrentState.LogicUpdate();
            UpdateAnimationDirectionParameters();
        }

        private void FixedUpdate()
        {
            FSM.CurrentState.PhysicsUpdate();
        }

        /// <summary>
        /// Updates MoveX and MoveY parameters in Animator so 2D Blend Trees select the correct directional clip.
        /// Actualiza los parámetros MoveX y MoveY en el Animator para que los Blend Trees 2D elijan el clip correcto.
        /// </summary>
        private void UpdateAnimationDirectionParameters()
        {
            if (Animator == null || MovementHandler == null) return;

            Vector2 dir = MovementHandler.FacingDirection;
            Animator.SetFloat("MoveX", dir.x);
            Animator.SetFloat("MoveY", dir.y);
        }
    }
}